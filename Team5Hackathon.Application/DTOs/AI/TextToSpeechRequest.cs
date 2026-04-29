namespace Team5Hackathon.Application.DTOs.AI;

public sealed class TextToSpeechRequest
{    
    public required string Text { get; init; }
    
    public string? VoiceName { get; init; }
}
