using System.Collections.Concurrent;

namespace Team5Hackathon.Application.Streaming;

public sealed class InMemoryAudioChunkBuffer
{
    private readonly int _maxChunksPerCall;
    private readonly ConcurrentDictionary<Guid, LinkedList<AudioChunkEnvelope>> _chunksByCall = new();
    private readonly object _sync = new();

    public InMemoryAudioChunkBuffer(int maxChunksPerCall)
    {
        _maxChunksPerCall = maxChunksPerCall;
    }

    public void Add(AudioChunkEnvelope chunk)
    {
        lock (_sync)
        {
            var list = _chunksByCall.GetOrAdd(chunk.CallId, _ => new LinkedList<AudioChunkEnvelope>());
            list.AddLast(chunk);
            while (list.Count > _maxChunksPerCall)
            {
                list.RemoveFirst();
            }
        }
    }
}
