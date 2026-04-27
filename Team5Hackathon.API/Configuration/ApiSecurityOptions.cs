namespace Team5Hackathon.API.Configuration;

public sealed class ApiSecurityOptions
{
    public const string SectionName = "ApiSecurity";
    public string HeaderName { get; set; } = "X-Api-Key";
    public string ApiKey { get; set; } = string.Empty;
}
