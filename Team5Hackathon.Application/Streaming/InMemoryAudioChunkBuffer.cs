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
<<<<<<< HEAD
=======

    public byte[] GetLatestAudioWindow(Guid callId, int maxChunksInWindow)
    {
        lock (_sync)
        {
            if (!_chunksByCall.TryGetValue(callId, out var list) || list.Count == 0)
            {
                return Array.Empty<byte>();
            }

            var chunks = list.Reverse().Take(maxChunksInWindow).Reverse().ToList();
            var totalBytes = chunks.Sum(x => x.ChunkBytes.Length);
            var merged = new byte[totalBytes];
            var offset = 0;

            foreach (var chunk in chunks)
            {
                Buffer.BlockCopy(chunk.ChunkBytes, 0, merged, offset, chunk.ChunkBytes.Length);
                offset += chunk.ChunkBytes.Length;
            }

            return merged;
        }
    }
>>>>>>> aad58e4df299d06879af545321d6fe668beb3017
}
