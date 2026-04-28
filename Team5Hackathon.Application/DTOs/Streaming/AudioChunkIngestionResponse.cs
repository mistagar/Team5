namespace Team5Hackathon.Application.DTOs.Streaming;

public sealed class AudioChunkIngestionResponse
{
    public bool Accepted { get; init; }
    public Guid CallId { get; init; }
    public long Sequence { get; init; }
    public required string CorrelationId { get; init; }
}
