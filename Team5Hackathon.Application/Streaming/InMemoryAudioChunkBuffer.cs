using System.Collections.Concurrent;

namespace Team5Hackathon.Application.Streaming;

public sealed class InMemoryAudioChunkBuffer
{
    private readonly int _maxChunksPerCall;
    private readonly ConcurrentDictionary<Guid, LinkedList<AudioChunkEnvelope>> _chunksByCall = new();
    // Keeps the very first chunk (EBML/WebM header) for each call so every merged
    // window starts with a valid container header.
    private readonly ConcurrentDictionary<Guid, AudioChunkEnvelope> _headerChunkByCall = new();
    private readonly object _sync = new();

    public InMemoryAudioChunkBuffer(int maxChunksPerCall)
    {
        _maxChunksPerCall = maxChunksPerCall;
    }

    public void Add(AudioChunkEnvelope chunk)
    {
        lock (_sync)
        {
            // Remember the very first chunk — it carries the WebM EBML/Tracks header.
            _headerChunkByCall.TryAdd(chunk.CallId, chunk);

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

            var windowChunks = list.Reverse().Take(maxChunksInWindow).Reverse().ToList();

            // Always prepend the header chunk so the merged bytes form a valid WebM file,
            // unless it is already the first element in the window.
            if (_headerChunkByCall.TryGetValue(callId, out var headerChunk)
                && (windowChunks.Count == 0 || windowChunks[0].Sequence != headerChunk.Sequence))
            {
                windowChunks.Insert(0, headerChunk);
            }

            var totalBytes = windowChunks.Sum(x => x.ChunkBytes.Length);
            var merged = new byte[totalBytes];
            var offset = 0;

            foreach (var chunk in windowChunks)
            {
                Buffer.BlockCopy(chunk.ChunkBytes, 0, merged, offset, chunk.ChunkBytes.Length);
                offset += chunk.ChunkBytes.Length;
            }

            return merged;
        }
    }

    /// <summary>Returns all buffered chunks for a call concatenated in sequence order — produces a complete, valid WebM file.</summary>
    public byte[] GetAllAudio(Guid callId)
    {
        lock (_sync)
        {
            if (!_chunksByCall.TryGetValue(callId, out var list) || list.Count == 0)
                return Array.Empty<byte>();

            var chunks = list.ToList(); // already in insertion (sequence) order
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

    /// <summary>Releases all buffered data for the given call (call ended).</summary>
    public void Remove(Guid callId)
    {
        lock (_sync)
        {
            _chunksByCall.TryRemove(callId, out _);
            _headerChunkByCall.TryRemove(callId, out _);
        }
    }

    /// <summary>
    /// Returns the MIME type and file extension for the call's audio, derived from
    /// the first buffered chunk (which carries the browser-reported ContentType).
    /// Defaults to "audio/webm" / ".webm" if no chunks exist yet.
    /// </summary>
    public (string MimeType, string FileExtension) GetCallFormat(Guid callId)
    {
        lock (_sync)
        {
            if (_headerChunkByCall.TryGetValue(callId, out var header))
            {
                // Strip codec parameters to get a bare MIME type.
                var baseMime = header.ContentType.Split(';')[0].Trim();
                var extension = baseMime switch
                {
                    "audio/ogg"  or "video/ogg"  => ".ogg",
                    "audio/mp4"  or "video/mp4"  => ".mp4",
                    "audio/mpeg"                 => ".mp3",
                    "audio/wav"                  => ".wav",
                    "audio/flac"                 => ".flac",
                    _                            => ".webm",  // audio/webm, video/webm, unknown
                };
                return (baseMime, extension);
            }

            return ("audio/webm", ".webm");
        }
    }
}
