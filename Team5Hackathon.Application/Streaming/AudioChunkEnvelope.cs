namespace Team5Hackathon.Application.Streaming;

public sealed class AudioChunkEnvelope
{
    public required Guid CallId { get; init; }
    public required long Sequence { get; init; }
    public required byte[] ChunkBytes { get; init; }
<<<<<<< HEAD
=======
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
>>>>>>> aad58e4df299d06879af545321d6fe668beb3017
    public required DateTimeOffset SentAtUtc { get; init; }
    public required string CorrelationId { get; init; }
}
