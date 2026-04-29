namespace Team5Hackathon.Application.DTOs.AI;

/// <summary>
/// Full result of the end-to-end complaint processing pipeline:
/// transcription ? analysis ? TTS (Pidgin + English).
/// </summary>
public sealed class ComplaintPipelineResponse
{
    public string TranscribedText { get; init; } = string.Empty;
    public ComplaintAnalysisResult Analysis { get; init; } = new();

    /// <summary>Base-64 encoded MP3 of the spoken Nigerian-Pidgin response.</summary>
    public string AudioResponseBase64 { get; init; } = string.Empty;

    /// <summary>Base-64 encoded MP3 of the spoken formal English response.</summary>
    public string EnglishAudioResponseBase64 { get; init; } = string.Empty;
}
