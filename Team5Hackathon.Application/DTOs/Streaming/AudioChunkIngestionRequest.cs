namespace Team5Hackathon.Application.DTOs.Streaming;

public sealed class AudioChunkIngestionRequest
{
    public Guid CallId { get; init; }
    public long Sequence { get; init; }
<<<<<<< HEAD
    public required string ChunkBase64 { get; init; }
=======
    public string? ChunkBase64 { get; init; }
    public string? AudioUrl { get; init; }
>>>>>>> aad58e4df299d06879af545321d6fe668beb3017
    public DateTimeOffset SentAtUtc { get; init; }
}
