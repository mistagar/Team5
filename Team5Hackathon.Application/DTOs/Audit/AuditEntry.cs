namespace Team5Hackathon.Application.DTOs.Audit;

public sealed class AuditEntry
{
    public required string Action { get; init; }
    public required string Outcome { get; init; }
    public string? EntityName { get; init; }
    public string? EntityId { get; init; }
    public string? Description { get; init; }
    public string? CorrelationId { get; init; }
    public string? PerformedBy { get; init; }
    public string? PerformedById { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}
