namespace Team5Hackathon.Application.Streaming;

public interface IAudioStreamQueue
{
    ValueTask QueueAsync(AudioChunkEnvelope chunk, CancellationToken cancellationToken);
    IAsyncEnumerable<AudioChunkEnvelope> ReadAllAsync(CancellationToken cancellationToken);
}
