namespace Team5Hackathon.Application.DTOs.Streaming;

public sealed class AudioChunkIngestionRequest
{
    public Guid CallId { get; init; }
    public long Sequence { get; init; }
    public string? ChunkBase64 { get; init; }
    public string? AudioUrl { get; init; }
    public DateTimeOffset SentAtUtc { get; init; }
}
