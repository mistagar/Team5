using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Team5Hackathon.Application.Configuration;

namespace Team5Hackathon.Application.Services;

public sealed class WhisperTranscriptionService : ITranscriptionService
{
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
        try
        {
            _logger.LogInformation("Starting transcription for file: {FileName}, Size: {Size} bytes", fileName, audioBytes.Length);

            if (string.IsNullOrWhiteSpace(_options.EndpointUrl)
                || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogError("Whisper transcription is not configured. EndpointUrl: '{EndpointUrl}', ApiKey exists: {ApiKeyExists}", 
                    _options.EndpointUrl, !string.IsNullOrWhiteSpace(_options.ApiKey));
                throw new InvalidOperationException("Whisper transcription service is not properly configured. Missing endpoint URL or API key.");
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, _options.EndpointUrl);
            request.Headers.TryAddWithoutValidation(_options.ApiKeyHeaderName, _options.ApiKey);

            using var form = new MultipartFormDataContent();
            using var audioContent = new ByteArrayContent(audioBytes);
            audioContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? _options.AudioContentType);
            form.Add(audioContent, _options.AudioFormFieldName, fileName);
            request.Content = form;

            _logger.LogDebug("Sending transcription request to: {EndpointUrl}", _options.EndpointUrl);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("Transcription response - Status: {StatusCode}, Body length: {BodyLength}", 
                response.StatusCode, rawBody?.Length ?? 0);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Whisper transcription failed with status code {StatusCode}. Response: {Response}", 
                    (int)response.StatusCode, rawBody);
                throw new HttpRequestException($"Transcription service returned {response.StatusCode}: {rawBody}");
            }

            if (string.IsNullOrWhiteSpace(rawBody))
            {
                _logger.LogWarning("Transcription service returned empty response");
                return null;
            }

            try
            {
                using var document = JsonDocument.Parse(rawBody);
                if (document.RootElement.TryGetProperty("text", out var textElement))
                {
                    var transcriptText = textElement.GetString();
                    _logger.LogInformation("Transcription completed successfully. Text length: {Length}", transcriptText?.Length ?? 0);
                    return transcriptText;
                }
                else
                {
                    _logger.LogWarning("Transcription response does not contain 'text' property. Raw response: {Response}", rawBody);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Could not parse transcription response as JSON. Treating as plain text. Response: {Response}", rawBody);
                // Some providers may return plain text; keep graceful fallback.
            }

            return rawBody;
        }
        catch (Exception ex) when (!(ex is HttpRequestException || ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Unexpected error during transcription");
            throw new InvalidOperationException($"Transcription service encountered an unexpected error: {ex.Message}", ex);
        }
    }
}
