using System.Threading.Channels;

namespace Team5Hackathon.Application.Streaming;

public sealed class InMemoryAudioStreamQueue : IAudioStreamQueue
{
    private readonly Channel<AudioChunkEnvelope> _channel = Channel.CreateUnbounded<AudioChunkEnvelope>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask QueueAsync(AudioChunkEnvelope chunk, CancellationToken cancellationToken)
    {
        return _channel.Writer.WriteAsync(chunk, cancellationToken);
    }

    public IAsyncEnumerable<AudioChunkEnvelope> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}
