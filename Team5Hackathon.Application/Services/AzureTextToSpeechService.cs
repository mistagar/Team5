using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Application.Services;

public sealed class AzureTextToSpeechService : ITextToSpeechService
{
    private const string DefaultVoice  = "en-NG-EzinneNeural";
    private const string OutputFormat  = "audio-16khz-128kbitrate-mono-mp3";

    // Derives the xml:lang locale from the voice name (e.g. "en-NG-EzinneNeural" ? "en-NG").
    // Falls back to "en-US" for unrecognised names.
    private static string VoiceToLang(string voiceName)
    {
        var parts = voiceName.Split('-');
        return parts.Length >= 2 ? $"{parts[0]}-{parts[1]}" : "en-US";
    }

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
        _speechKey   = configuration["AzureSpeech:Key"]
            ?? throw new InvalidOperationException("AzureSpeech:Key is required.");
        _speechRegion = configuration["AzureSpeech:Region"]
            ?? throw new InvalidOperationException("AzureSpeech:Region is required.");
        _logger = logger;
    }

    public async Task<TextToSpeechResponse> SynthesiseAsync(
        TextToSpeechRequest request,
        CancellationToken cancellationToken = default)
    {
        var voiceName = string.IsNullOrWhiteSpace(request.VoiceName) ? DefaultVoice : request.VoiceName;
        var lang      = VoiceToLang(voiceName);

        var ssml = $"""
            <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='{lang}'>
                <voice xml:lang='{lang}' xml:gender='Female' name='{voiceName}'>
                    {SecurityElement.Escape(request.Text)}
                </voice>
            </speak>
            """;

        var url = $"https://{_speechRegion}.tts.speech.microsoft.com/cognitiveservices/v1";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
        httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", _speechKey);
        httpRequest.Headers.Add("User-Agent", "Team5Hackathon");
        // Required: tells Azure TTS which audio format to encode and return.
        httpRequest.Headers.Add("X-Microsoft-OutputFormat", OutputFormat);

        httpRequest.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

        _logger.LogInformation(
            "Synthesising speech with voice '{Voice}', text length {Length}.",
            voiceName, request.Text.Length);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "TTS request failed with status {StatusCode}: {Error}",
                response.StatusCode, errorContent);
            throw new InvalidOperationException(
                $"TTS synthesis failed: {response.StatusCode} - {errorContent}");
        }

        var audioBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        return new TextToSpeechResponse { AudioBase64 = Convert.ToBase64String(audioBytes) };
    }
}
