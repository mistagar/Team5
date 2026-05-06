using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Team5Hackathon.Application.Configuration;

namespace Team5Hackathon.Application.Services;

public sealed class WhisperTranscriptionService : ITranscriptionService
{
    private const int MaxRetries = 4;

    // Global semaphore: limits concurrent Whisper requests to avoid saturating the S0 rate limit.
    private static readonly SemaphoreSlim _concurrencyGate = new(3, 3);

    // Maps the bare MIME type (codec params stripped) to a Whisper-supported file extension.
    // Covers every format produced by Chrome, Edge, Firefox, and Safari MediaRecorder.
    private static readonly Dictionary<string, string> MimeToExtension =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["audio/webm"]  = "webm",   // Chrome, Edge, newer Firefox
            ["video/webm"]  = "webm",   // Some browsers report video/webm for audio-only
            ["audio/ogg"]   = "ogg",    // Firefox (Opus/Vorbis in Ogg container)
            ["video/ogg"]   = "ogg",
            ["audio/mp4"]   = "mp4",    // Safari (AAC-LC in MP4/M4A container)
            ["video/mp4"]   = "mp4",
            ["audio/mpeg"]  = "mp3",
            ["audio/mp3"]   = "mp3",
            ["audio/wav"]   = "wav",
            ["audio/wave"]  = "wav",
            ["audio/flac"]  = "flac",
            ["audio/x-m4a"] = "m4a",
            ["audio/aac"]   = "m4a",
        };

    private readonly HttpClient _httpClient;
    private readonly WhisperTranscriptionOptions _options;
    private readonly ILogger<WhisperTranscriptionService> _logger;

    public WhisperTranscriptionService(
        HttpClient httpClient,
        IOptions<WhisperTranscriptionOptions> options,
        ILogger<WhisperTranscriptionService> logger)
    {
        _httpClient = httpClient;
        _options    = options.Value;
        _logger     = logger;
    }

    public async Task<string?> TranscribeAsync(
        byte[] audioBytes,
        string fileName,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Starting transcription for file: {FileName}, Size: {Size} bytes",
                fileName, audioBytes.Length);

            if (string.IsNullOrWhiteSpace(_options.EndpointUrl)
                || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogError(
                    "Whisper transcription is not configured. EndpointUrl: '{EndpointUrl}', ApiKey exists: {ApiKeyExists}",
                    _options.EndpointUrl, !string.IsNullOrWhiteSpace(_options.ApiKey));
                throw new InvalidOperationException(
                    "Whisper transcription service is not properly configured. Missing endpoint URL or API key.");
            }

            // Normalise the content type: strip codec parameters and derive a safe
            // file extension so Whisper always receives a recognised container format
            // regardless of which browser produced the audio.
            var (safeMime, safeExtension) = NormaliseContentType(contentType, fileName);

            // Ensure the file name carries the correct extension so Whisper can
            // identify the container even if it ignores the Content-Type header.
            var safeFileName = Path.ChangeExtension(
                Path.GetFileNameWithoutExtension(fileName) is { Length: > 0 } stem ? stem : "audio",
                safeExtension);

            await _concurrencyGate.WaitAsync(cancellationToken);
            try
            {
                return await TranscribeWithRetryAsync(
                    audioBytes, safeFileName, safeMime, cancellationToken);
            }
            finally
            {
                _concurrencyGate.Release();
            }
        }
        catch (Exception ex) when (ex is not HttpRequestException and not InvalidOperationException and not OperationCanceledException)
        {
            _logger.LogError(ex, "Unexpected error during transcription");
            throw new InvalidOperationException(
                $"Transcription service encountered an unexpected error: {ex.Message}", ex);
        }
    }

    // -------------------------------------------------------------------------

    private async Task<string?> TranscribeWithRetryAsync(
        byte[] audioBytes,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromSeconds(2);

        for (var attempt = 0; attempt <= MaxRetries; attempt++)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            using var request = new HttpRequestMessage(HttpMethod.Post, _options.EndpointUrl);
            request.Headers.TryAddWithoutValidation(_options.ApiKeyHeaderName, _options.ApiKey);

            using var form = new MultipartFormDataContent();
            using var audioContent = new ByteArrayContent(audioBytes);
            // Use Parse() so that values with parameters (e.g. "audio/webm;codecs=opus")
            // are accepted. At this point contentType is already the bare MIME, but
            // Parse() is safer than the constructor regardless.
            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            form.Add(audioContent, _options.AudioFormFieldName, fileName);
            request.Content = form;

            _logger.LogDebug(
                "Sending transcription request to: {EndpointUrl} (attempt {Attempt}/{MaxRetries})",
                _options.EndpointUrl, attempt + 1, MaxRetries + 1);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var transcript = ParseTranscript(rawBody);
                _logger.LogInformation(
                    "Transcription completed successfully. Text length: {Length}",
                    transcript?.Length ?? 0);
                return transcript;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests && attempt < MaxRetries)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? delay;

                _logger.LogWarning(
                    "Whisper transcription failed with status code {StatusCode}. Body: {Body}",
                    (int)response.StatusCode, rawBody);
                _logger.LogInformation(
                    "Rate-limited by Whisper. Attempt {Attempt}/{MaxRetries}. Retrying in {Delay}s.",
                    attempt + 1, MaxRetries, retryAfter.TotalSeconds);

                try
                {
                    await Task.Delay(retryAfter, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation(
                        "Whisper retry cancelled during backoff wait (attempt {Attempt}/{MaxRetries}).",
                        attempt + 1, MaxRetries);
                    return null;
                }

                delay *= 2;
                continue;
            }

            _logger.LogError(
                "Whisper transcription failed with status code {StatusCode}. Response: {Response}",
                (int)response.StatusCode, rawBody);
            throw new HttpRequestException(
                $"Transcription service returned {response.StatusCode}: {rawBody}");
        }

        return null;
    }

    /// <summary>
    /// Strips codec parameters from the MIME type and returns a bare MIME type +
    /// a Whisper-supported file extension.  Handles every format emitted by
    /// Chrome, Edge, Firefox, and Safari MediaRecorder as well as plain file uploads.
    /// </summary>
    private (string Mime, string Extension) NormaliseContentType(
        string? contentType, string fileName)
    {
        // 1. Strip parameters: "audio/webm;codecs=opus" ? "audio/webm"
        var baseMime = (contentType ?? string.Empty).Split(';')[0].Trim();

        if (!string.IsNullOrWhiteSpace(baseMime)
            && MimeToExtension.TryGetValue(baseMime, out var extFromMime))
        {
            return (baseMime, extFromMime);
        }

        // 2. Fall back to the file extension when the MIME type is absent or unrecognised.
        var fileExt = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(fileExt))
        {
            // Find the first MIME that maps to this extension.
            var mimeFromExt = MimeToExtension
                .FirstOrDefault(kv => kv.Value.Equals(fileExt, StringComparison.OrdinalIgnoreCase))
                .Key;

            if (mimeFromExt is not null)
                return (mimeFromExt, fileExt);
        }

        // 3. Ultimate default: WebM — supported by Chrome, Edge, and newer Firefox.
        _logger.LogWarning(
            "Could not determine audio format from ContentType '{ContentType}' or file '{FileName}'. Defaulting to audio/webm.",
            contentType, fileName);
        return (_options.AudioContentType, "webm");
    }

    private string? ParseTranscript(string rawBody)
    {
        if (string.IsNullOrWhiteSpace(rawBody))
            return null;

        try
        {
            using var document = JsonDocument.Parse(rawBody);
            if (document.RootElement.TryGetProperty("text", out var textElement))
                return textElement.GetString();

            _logger.LogWarning(
                "Transcription response does not contain 'text' property. Raw: {Response}", rawBody);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex,
                "Could not parse transcription response as JSON. Treating as plain text.");
        }

        return rawBody;
    }
}
