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
    private static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(32);

    // Global semaphore — limits how many Whisper requests fly concurrently
    // across all calls to avoid saturating the S0 rate limit.
    private static readonly SemaphoreSlim _concurrencyGate = new(3, 3);

    private readonly HttpClient _httpClient;
    private readonly WhisperTranscriptionOptions _options;
    private readonly ILogger<WhisperTranscriptionService> _logger;

    public WhisperTranscriptionService(
        HttpClient httpClient,
        IOptions<WhisperTranscriptionOptions> options,
        ILogger<WhisperTranscriptionService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string?> TranscribeAsync(
        byte[] audioBytes,
        string fileName,
        string? contentType = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.EndpointUrl)
            || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("Whisper transcription is not configured. Endpoint or API key is missing.");
            return null;
        }

        await _concurrencyGate.WaitAsync(cancellationToken);
        try
        {
            return await TranscribeWithRetryAsync(audioBytes, fileName, contentType, cancellationToken);
        }
        finally
        {
            _concurrencyGate.Release();
        }
    }

    private async Task<string?> TranscribeWithRetryAsync(
        byte[] audioBytes,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken)
    {
        var delay = TimeSpan.FromSeconds(2);

        for (var attempt = 0; attempt <= MaxRetries; attempt++)
        {
            // Bail out cleanly if the app is shutting down or the caller cancelled.
            if (cancellationToken.IsCancellationRequested)
                return null;

            using var request = new HttpRequestMessage(HttpMethod.Post, _options.EndpointUrl);
            request.Headers.TryAddWithoutValidation(_options.ApiKeyHeaderName, _options.ApiKey);

            using var form = new MultipartFormDataContent();
            using var audioContent = new ByteArrayContent(audioBytes);

            var rawContentType = contentType ?? _options.AudioContentType;
            audioContent.Headers.ContentType = MediaTypeHeaderValue.Parse(rawContentType);

            form.Add(audioContent, _options.AudioFormFieldName, fileName);
            request.Content = form;

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
                return ParseTranscript(rawBody);

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
                    // App is shutting down or the call was aborted — stop retrying quietly.
                    _logger.LogInformation(
                        "Whisper retry cancelled during backoff wait (attempt {Attempt}/{MaxRetries}).",
                        attempt + 1, MaxRetries);
                    return null;
                }

                delay *= 2;
                continue;
            }

            _logger.LogWarning(
                "Whisper transcription failed with status code {StatusCode}. Body: {Body}",
                (int)response.StatusCode, rawBody);
            return null;
        }

        return null;
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
        }
        catch (JsonException)
        {
            // Some providers may return plain text; fall through.
        }

        return rawBody;
    }
}
