namespace Team5Hackathon.Application.Streaming;

public sealed class AudioChunkEnvelope
{
    public required Guid CallId { get; init; }
    public required long Sequence { get; init; }
    public required byte[] ChunkBytes { get; init; }
    public required DateTimeOffset SentAtUtc { get; init; }
    public required string CorrelationId { get; init; }
}
