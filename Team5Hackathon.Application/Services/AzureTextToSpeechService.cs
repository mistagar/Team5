using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Application.Services;

public sealed class AzureTextToSpeechService : ITextToSpeechService
{
    private const string DefaultVoice = "en-NG-EzinneNeural";

    private readonly HttpClient _httpClient;
    private readonly string _speechKey;
    private readonly string _speechRegion;
    private readonly ILogger<AzureTextToSpeechService> _logger;

    public AzureTextToSpeechService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AzureTextToSpeechService> logger)
    {
        _httpClient = httpClient;
        _speechKey = configuration["AzureSpeech:Key"]
            ?? throw new InvalidOperationException("AzureSpeech:Key is required.");
        _speechRegion = configuration["AzureSpeech:Region"]
            ?? throw new InvalidOperationException("AzureSpeech:Region is required.");
        _logger = logger;
    }

    public async Task<TextToSpeechResponse> SynthesiseAsync(
        TextToSpeechRequest request,
        CancellationToken cancellationToken = default)
    {
        var voiceName = request.VoiceName ?? DefaultVoice;

        _logger.LogInformation(
            "Synthesising speech with voice '{Voice}', text length {Length}.",
            voiceName,
            request.Text.Length);

        var ssml = BuildSsml(request.Text, voiceName);

        var url = $"https://{_speechRegion}.tts.speech.microsoft.com/cognitiveservices/v1";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", _speechKey);
        httpRequest.Headers.Add("User-Agent", "Team5Hackathon");
        httpRequest.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
        httpRequest.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "TTS REST API failed. Status: {Status}. Body: {Body}",
                response.StatusCode, error);
            throw new InvalidOperationException(
                $"TTS REST API returned {(int)response.StatusCode}: {error}");
        }

        var audioBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return new TextToSpeechResponse { AudioBase64 = Convert.ToBase64String(audioBytes) };
    }

    private static string BuildSsml(string text, string voiceName)
    {
        // Escape XML special characters in the text
        var escaped = text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");

        return $"""
            <speak version='1.0' xml:lang='en-US'>
              <voice name='{voiceName}'>
                {escaped}
              </voice>
            </speak>
            """;
    }
}
