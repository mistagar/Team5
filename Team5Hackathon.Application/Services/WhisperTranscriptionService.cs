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
        if (string.IsNullOrWhiteSpace(_options.EndpointUrl)
            || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("Whisper transcription is not configured. Endpoint or API key is missing.");
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.EndpointUrl);
        request.Headers.TryAddWithoutValidation(_options.ApiKeyHeaderName, _options.ApiKey);

        using var form = new MultipartFormDataContent();
        using var audioContent = new ByteArrayContent(audioBytes);
        audioContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? _options.AudioContentType);
        form.Add(audioContent, _options.AudioFormFieldName, fileName);
        request.Content = form;

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var rawBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "Whisper transcription failed with status code {StatusCode}. Body: {Body}",
                (int)response.StatusCode,
                rawBody);
            return null;
        }

        if (string.IsNullOrWhiteSpace(rawBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(rawBody);
            if (document.RootElement.TryGetProperty("text", out var textElement))
            {
                return textElement.GetString();
            }
        }
        catch (JsonException)
        {
            // Some providers may return plain text; keep graceful fallback.
        }

        return rawBody;
    }
}
