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
    private static readonly Dictionary<string, string> ExtensionContentTypeMap = new(StringComparer.OrdinalIgnoreCase)
    {
        [".mp3"] = "audio/mpeg",
        [".wav"] = "audio/wav",
        [".webm"] = "audio/webm",
        [".ogg"] = "audio/ogg",
        [".m4a"] = "audio/mp4",
        [".mp4"] = "audio/mp4",
        [".flac"] = "audio/flac",
        [".oga"] = "audio/ogg",
        [".mpeg"] = "audio/mpeg",
        [".mpga"] = "audio/mpeg"
    };
    private readonly ConcurrentDictionary<Guid, long> _lastSequenceByCall = new();
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
            var chunkBytes = source.ChunkBytes;
            ValidateChunkSize(chunkBytes.Length);
            ValidateSequenceOrdering(request.CallId, request.Sequence);

            await _audioStreamQueue.QueueAsync(
                new AudioChunkEnvelope
                {
                    CallId = request.CallId,
                    Sequence = request.Sequence,
                    ChunkBytes = chunkBytes,
                    FileName = source.FileName,
                    ContentType = source.ContentType,
                    SentAtUtc = request.SentAtUtc,
                    CorrelationId = correlationId
                },
                cancellationToken);

            return new AudioChunkIngestionResponse
            {
                CallId = request.CallId,
                Sequence = request.Sequence,
                Accepted = true,
                CorrelationId = correlationId
            };
        }
        catch (InvalidAudioChunkException ex)
        {
            await _auditService.RecordAsync(
                new AuditEntry
                {
                    Action = AuditActionNames.StreamAudioChunkIngest,
                    Outcome = "failed",
                    EntityName = "Call",
                    EntityId = request.CallId == Guid.Empty ? null : request.CallId.ToString(),
                    Description = ex.Message,
                    CorrelationId = correlationId
                },
                cancellationToken);

            throw;
        }
    }

    private static void ValidateRequest(AudioChunkIngestionRequest request)
    {
        if (request.CallId == Guid.Empty)
        {
            throw new InvalidAudioChunkException("CallId is required.");
        }

        if (request.Sequence <= 0)
        {
            throw new InvalidAudioChunkException("Sequence must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.ChunkBase64) && string.IsNullOrWhiteSpace(request.AudioUrl))
        {
            throw new InvalidAudioChunkException("Provide either ChunkBase64 or AudioUrl.");
        }
    }

    private static async Task<(byte[] ChunkBytes, string FileName, string ContentType)> ResolveAudioSourceAsync(
        AudioChunkIngestionRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.ChunkBase64))
        {
            return (DecodeChunk(request.ChunkBase64), "chunk.webm", "audio/webm");
        }

        return await DownloadAudioFromUrlAsync(request.AudioUrl!, cancellationToken);
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
        {
            throw new InvalidAudioChunkException("Decoded audio chunk cannot be empty.");
        }

        if (sizeBytes > MaxChunkSizeBytes)
        {
            throw new InvalidAudioChunkException("Decoded audio chunk exceeds the maximum allowed size.");
        }
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
        {
            throw new InvalidAudioChunkException("AudioUrl format is not supported.");
        }

        byte[] bytes;
        try
        {
            bytes = await HttpClient.GetByteArrayAsync(uri, cancellationToken);
        }
        catch (Exception)
        {
            throw new InvalidAudioChunkException("Unable to download audio from AudioUrl.");
        }

        var fileName = Path.GetFileName(uri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = $"audio{extension}";
        }

        return (bytes, fileName, contentType);
    }

    private void ValidateSequenceOrdering(Guid callId, long sequence)
    {
        var currentLast = _lastSequenceByCall.AddOrUpdate(callId, sequence, (_, existing) =>
        {
            if (sequence <= existing)
            {
                throw new InvalidAudioChunkException("Sequence must be strictly increasing per call.");
            }

            return sequence;
        });

        if (currentLast != sequence)
        {
            throw new InvalidAudioChunkException("Invalid sequence state.");
        }
    }
}
