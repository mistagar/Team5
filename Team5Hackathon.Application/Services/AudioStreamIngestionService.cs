using System.Collections.Concurrent;
using System.Net.Http;
using Team5Hackathon.Application.Constants;
using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Application.DTOs.Streaming;
using Team5Hackathon.Application.Exceptions;
using Team5Hackathon.Application.Streaming;

namespace Team5Hackathon.Application.Services;

public sealed class AudioStreamIngestionService : IAudioStreamIngestionService
{
    private const int MaxChunkSizeBytes = 10_485_760; // 10 MB

    private static readonly HttpClient HttpClient = new();

    // Maps the base MIME type (parameters stripped) to a canonical file extension.
    // Covers every format Whisper accepts across Chrome, Firefox, Edge, and Safari.
    private static readonly Dictionary<string, string> MimeTypeExtensionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["audio/webm"]  = ".webm",  // Chrome, Edge, newer Firefox
        ["audio/ogg"]   = ".ogg",   // Firefox (Opus/Vorbis in Ogg container)
        ["audio/mp4"]   = ".mp4",   // Safari (AAC in MP4/M4A container)
        ["audio/mpeg"]  = ".mp3",
        ["audio/wav"]   = ".wav",
        ["audio/flac"]  = ".flac",
        ["video/webm"]  = ".webm",  // Some browsers report video/webm even for audio-only
        ["video/mp4"]   = ".mp4",
    };

    // Reverse map: extension ? MIME type, used when resolving URLs.
    private static readonly Dictionary<string, string> ExtensionContentTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        [".mp3"]  = "audio/mpeg",
        [".wav"]  = "audio/wav",
        [".webm"] = "audio/webm",
        [".ogg"]  = "audio/ogg",
        [".oga"]  = "audio/ogg",
        [".m4a"]  = "audio/mp4",
        [".mp4"]  = "audio/mp4",
        [".flac"] = "audio/flac",
        [".mpeg"] = "audio/mpeg",
        [".mpga"] = "audio/mpeg",
    };

    private readonly ConcurrentDictionary<Guid, long> _lastSequenceByCall = new();
    // Remembers the MIME type / extension decided for the first chunk of each call
    // so all subsequent chunks use the same format.
    private readonly ConcurrentDictionary<Guid, (string ContentType, string Extension)> _formatByCall = new();

    private readonly IAudioStreamQueue _audioStreamQueue;
    private readonly IAuditService _auditService;

    public AudioStreamIngestionService(IAudioStreamQueue audioStreamQueue, IAuditService auditService)
    {
        _audioStreamQueue = audioStreamQueue;
        _auditService = auditService;
    }

    public async Task<AudioChunkIngestionResponse> IngestChunkAsync(
        AudioChunkIngestionRequest request,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRequest(request);

            var source = await ResolveAudioSourceAsync(request, cancellationToken);
            ValidateChunkSize(source.ChunkBytes.Length);
            ValidateSequenceOrdering(request.CallId, request.Sequence);

            await _audioStreamQueue.QueueAsync(
                new AudioChunkEnvelope
                {
                    CallId      = request.CallId,
                    Sequence    = request.Sequence,
                    ChunkBytes  = source.ChunkBytes,
                    FileName    = source.FileName,
                    ContentType = source.ContentType,
                    SentAtUtc   = request.SentAtUtc,
                    CorrelationId = correlationId
                },
                cancellationToken);

            return new AudioChunkIngestionResponse
            {
                CallId        = request.CallId,
                Sequence      = request.Sequence,
                Accepted      = true,
                CorrelationId = correlationId
            };
        }
        catch (InvalidAudioChunkException ex)
        {
            await _auditService.RecordAsync(
                new AuditEntry
                {
                    Action      = AuditActionNames.StreamAudioChunkIngest,
                    Outcome     = "failed",
                    EntityName  = "Call",
                    EntityId    = request.CallId == Guid.Empty ? null : request.CallId.ToString(),
                    Description = ex.Message,
                    CorrelationId = correlationId
                },
                cancellationToken);
            throw;
        }
    }

    // -------------------------------------------------------------------------

    private static void ValidateRequest(AudioChunkIngestionRequest request)
    {
        if (request.CallId == Guid.Empty)
            throw new InvalidAudioChunkException("CallId is required.");

        if (request.Sequence <= 0)
            throw new InvalidAudioChunkException("Sequence must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.ChunkBase64) && string.IsNullOrWhiteSpace(request.AudioUrl))
            throw new InvalidAudioChunkException("Provide either ChunkBase64 or AudioUrl.");
    }

    private async Task<(byte[] ChunkBytes, string FileName, string ContentType)> ResolveAudioSourceAsync(
        AudioChunkIngestionRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.ChunkBase64))
        {
            var bytes = DecodeChunk(request.ChunkBase64);
            var (contentType, extension) = ResolveFormatForCall(request.CallId, request.MimeType);
            return (bytes, $"chunk{extension}", contentType);
        }

        return await DownloadAudioFromUrlAsync(request.AudioUrl!, cancellationToken);
    }

    /// <summary>
    /// Returns the canonical content-type and file extension to use for chunks of this call.
    /// The format is locked in on the first chunk (where the browser supplies MimeType)
    /// and reused for every subsequent chunk so all chunks have a consistent format.
    /// </summary>
    private (string ContentType, string Extension) ResolveFormatForCall(Guid callId, string? mimeType)
    {
        return _formatByCall.GetOrAdd(callId, _ =>
        {
            if (!string.IsNullOrWhiteSpace(mimeType))
            {
                // Strip codec parameters: "audio/webm;codecs=opus" ? "audio/webm"
                var baseMime = mimeType.Split(';')[0].Trim();

                if (MimeTypeExtensionMap.TryGetValue(baseMime, out var ext))
                    return (baseMime, ext);
            }

            // Default: WebM is supported by Chrome, Edge, and newer Firefox.
            return ("audio/webm", ".webm");
        });
    }

    private static byte[] DecodeChunk(string chunkBase64)
    {
        try
        {
            return Convert.FromBase64String(chunkBase64);
        }
        catch (FormatException)
        {
            throw new InvalidAudioChunkException("ChunkBase64 is not a valid Base64 value.");
        }
    }

    private static void ValidateChunkSize(int sizeBytes)
    {
        if (sizeBytes == 0)
            throw new InvalidAudioChunkException("Decoded audio chunk cannot be empty.");

        if (sizeBytes > MaxChunkSizeBytes)
            throw new InvalidAudioChunkException("Decoded audio chunk exceeds the maximum allowed size.");
    }

    private static async Task<(byte[] ChunkBytes, string FileName, string ContentType)> DownloadAudioFromUrlAsync(
        string audioUrl,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(audioUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidAudioChunkException("AudioUrl must be a valid absolute HTTP/HTTPS URL.");
        }

        var extension = Path.GetExtension(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension) || !ExtensionContentTypeMap.TryGetValue(extension, out var contentType))
            throw new InvalidAudioChunkException("AudioUrl format is not supported.");

        byte[] bytes;
        try
        {
            bytes = await HttpClient.GetByteArrayAsync(uri, cancellationToken);
        }
        catch
        {
            throw new InvalidAudioChunkException("Unable to download audio from AudioUrl.");
        }

        var fileName = Path.GetFileName(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = $"audio{extension}";

        return (bytes, fileName, contentType);
    }

    private void ValidateSequenceOrdering(Guid callId, long sequence)
    {
        var currentLast = _lastSequenceByCall.AddOrUpdate(callId, sequence, (_, existing) =>
        {
            if (sequence <= existing)
                throw new InvalidAudioChunkException("Sequence must be strictly increasing per call.");
            return sequence;
        });

        if (currentLast != sequence)
            throw new InvalidAudioChunkException("Invalid sequence state.");
    }
}
