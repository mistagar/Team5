using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Application.Services;

public sealed class AzureTextToSpeechService : ITextToSpeechService
{
    private const string DefaultVoice = "en-NG-EzinneNeural";

    private readonly string _speechKey;
    private readonly string _speechRegion;
    private readonly ILogger<AzureTextToSpeechService> _logger;

    public AzureTextToSpeechService(
        IConfiguration configuration,
        ILogger<AzureTextToSpeechService> logger)
    {
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
        var speechConfig = SpeechConfig.FromSubscription(_speechKey, _speechRegion);
        speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz32KBitRateMonoMp3);
        speechConfig.SpeechSynthesisVoiceName = request.VoiceName ?? DefaultVoice;

        // Use in-memory stream output
        using var audioStream = AudioOutputStream.CreatePullStream();
        using var audioConfig = AudioConfig.FromStreamOutput(audioStream);

        using var synthesizer = new SpeechSynthesizer(speechConfig, audioConfig);

        _logger.LogInformation(
            "Synthesising speech with voice '{Voice}', text length {Length}.",
            speechConfig.SpeechSynthesisVoiceName,
            request.Text.Length);

        var result = await synthesizer.SpeakTextAsync(request.Text);

        if (result.Reason == ResultReason.Canceled)
        {
            var details = SpeechSynthesisCancellationDetails.FromResult(result);
            _logger.LogError(
                "TTS cancelled. Reason: {Reason}. Error: {Error}",
                details.Reason,
                details.ErrorDetails);
            throw new InvalidOperationException($"TTS cancelled: {details.ErrorDetails}");
        }

        var audioBytes = result.AudioData;
        var base64 = Convert.ToBase64String(audioBytes);

        return new TextToSpeechResponse { AudioBase64 = base64 };
    }
}
