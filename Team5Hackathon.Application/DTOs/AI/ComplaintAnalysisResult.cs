using System.Text.Json.Serialization;

namespace Team5Hackathon.Application.DTOs.AI;

/// <summary>
/// Structured result returned by the GPT complaint-analysis prompt.
/// </summary>
public sealed class ComplaintAnalysisResult
{
    /// <summary>The original transcribed text that was analysed.</summary>
    [JsonPropertyName("transcribed_text")]
    public string TranscribedText { get; set; } = string.Empty;

    
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

   
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    
    [JsonPropertyName("sentiment")]
    public string Sentiment { get; set; } = string.Empty;

    
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;

   
    [JsonPropertyName("english_response")]
    public string EnglishResponse { get; set; } = string.Empty;
}
