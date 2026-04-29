namespace Team5Hackathon.Application.Configuration;

public sealed class WhisperTranscriptionOptions
{
    public const string SectionName = "WhisperTranscription";
    public string EndpointUrl { get; set; } = string.Empty;
    public string ApiKeyHeaderName { get; set; } = "api-key";
    public string ApiKey { get; set; } = string.Empty;
    public string AudioFormFieldName { get; set; } = "file";
    public string AudioContentType { get; set; } = "audio/webm";
}
