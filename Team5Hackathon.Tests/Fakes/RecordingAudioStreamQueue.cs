using Team5Hackathon.Application.Streaming;

namespace Team5Hackathon.Tests.Fakes;

internal sealed class RecordingAudioStreamQueue : IAudioStreamQueue
{
    public List<AudioChunkEnvelope> Queued { get; } = new();

    public ValueTask QueueAsync(AudioChunkEnvelope chunk, CancellationToken cancellationToken)
    {
        Queued.Add(chunk);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<AudioChunkEnvelope> ReadAllAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        yield break;
    }
}
