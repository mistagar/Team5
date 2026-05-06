using System.Collections.Concurrent;

namespace Team5Hackathon.Application.Streaming;

public sealed class InMemoryAudioChunkBuffer
{
    private readonly int _maxChunksPerCall;
    private readonly ConcurrentDictionary<Guid, LinkedList<AudioChunkEnvelope>> _chunksByCall = new();
    // Keeps the very first chunk (WebM init / EBML segment) per call so it can always be prepended.
    private readonly ConcurrentDictionary<Guid, AudioChunkEnvelope> _initChunkByCall = new();
    private readonly object _sync = new();

    public InMemoryAudioChunkBuffer(int maxChunksPerCall)
    {
        _maxChunksPerCall = maxChunksPerCall;
    }

    public void Add(AudioChunkEnvelope chunk)
    {
        lock (_sync)
        {
            // Store the very first chunk received for this call as the init segment.
            _initChunkByCall.TryAdd(chunk.CallId, chunk);

            var list = _chunksByCall.GetOrAdd(chunk.CallId, _ => new LinkedList<AudioChunkEnvelope>());
            list.AddLast(chunk);
            while (list.Count > _maxChunksPerCall)
            {
                list.RemoveFirst();
            }
        }
    }

    public byte[] GetLatestAudioWindow(Guid callId, int maxChunksInWindow)
    {
        lock (_sync)
        {
            if (!_chunksByCall.TryGetValue(callId, out var list) || list.Count == 0)
            {
                return Array.Empty<byte>();
            }

            // Always start with the init segment (first chunk which carries the WebM EBML header).
            _initChunkByCall.TryGetValue(callId, out var initChunk);

            // Take the most-recent (maxChunksInWindow - 1) chunks for content, leaving one slot for init.
            int contentSlots = initChunk is not null ? maxChunksInWindow - 1 : maxChunksInWindow;
            contentSlots = Math.Max(contentSlots, 1);

            var recentChunks = list.Reverse()
                                   .Take(contentSlots)
                                   .Reverse()
                                   .ToList();

            // Build the final window: init segment first (if not already included), then recent content.
            IEnumerable<AudioChunkEnvelope> windowChunks;
            if (initChunk is null || (recentChunks.Count > 0 && recentChunks[0].Sequence == initChunk.Sequence))
            {
                windowChunks = recentChunks;
            }
            else
            {
                windowChunks = new[] { initChunk }.Concat(recentChunks);
            }

            return Merge(windowChunks);
        }
    }

    /// <summary>
    /// Returns every buffered chunk for the call concatenated in sequence order,
    /// producing a complete valid audio file for final transcription.
    /// </summary>
    public byte[] GetAllAudio(Guid callId)
    {
        lock (_sync)
        {
            if (!_chunksByCall.TryGetValue(callId, out var list) || list.Count == 0)
            {
                return Array.Empty<byte>();
            }

            return Merge(list);
        }
    }

    /// <summary>
    /// Returns the canonical MIME type and file extension for the call's audio,
    /// derived from the first buffered chunk's ContentType.
    /// Defaults to "audio/webm" / ".webm" when no chunks exist yet.
    /// </summary>
    public (string MimeType, string FileExtension) GetCallFormat(Guid callId)
    {
        lock (_sync)
        {
            if (_initChunkByCall.TryGetValue(callId, out var initChunk))
            {
                var baseMime = initChunk.ContentType.Split(';')[0].Trim();
                var extension = baseMime switch
                {
                    "audio/ogg" or "video/ogg" => ".ogg",
                    "audio/mp4" or "video/mp4" => ".mp4",
                    "audio/mpeg"               => ".mp3",
                    "audio/wav"                => ".wav",
                    "audio/flac"               => ".flac",
                    _                          => ".webm",
                };
                return (baseMime, extension);
            }
            return ("audio/webm", ".webm");
        }
    }

    /// <summary>Clears all buffered data for a call once it has ended.</summary>
    public void Remove(Guid callId)
    {
        lock (_sync)
        {
            _chunksByCall.TryRemove(callId, out _);
            _initChunkByCall.TryRemove(callId, out _);
        }
    }

    private static byte[] Merge(IEnumerable<AudioChunkEnvelope> chunks)
    {
        var list = chunks as IList<AudioChunkEnvelope> ?? chunks.ToList();
        var totalBytes = list.Sum(x => x.ChunkBytes.Length);
        var merged = new byte[totalBytes];
        var offset = 0;
        foreach (var chunk in list)
        {
            Buffer.BlockCopy(chunk.ChunkBytes, 0, merged, offset, chunk.ChunkBytes.Length);
            offset += chunk.ChunkBytes.Length;
        }
        return merged;
    }
}
