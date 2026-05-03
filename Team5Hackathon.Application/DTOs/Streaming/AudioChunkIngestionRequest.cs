namespace Team5Hackathon.Application.DTOs.Streaming;

public sealed class AudioChunkIngestionRequest
{
    public Guid CallId { get; init; }
    public long Sequence { get; init; }
    public string? ChunkBase64 { get; init; }
    public string? AudioUrl { get; init; }
    public DateTimeOffset SentAtUtc { get; init; }

    /// <summary>
    /// MIME type reported by the browser's MediaRecorder (e.g. "audio/webm;codecs=opus",
    /// "audio/mp4", "audio/ogg;codecs=opus").  Required when sending ChunkBase64 so the
    /// server knows the actual container format.  Ignored when AudioUrl is used (format
    /// is inferred from the URL file extension).
    /// </summary>
    public string? MimeType { get; init; }
}
