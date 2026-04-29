namespace Team5Hackathon.Application.DTOs.AI;

public sealed class TextToSpeechResponse
{
   
    public required string AudioBase64 { get; init; }
    
    public string ContentType { get; init; } = "audio/mpeg";
}
