using Team5Hackathon.Application.DTOs.Streaming;
using Team5Hackathon.Application.Exceptions;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.Tests.Fakes;

internal sealed class StubAudioStreamIngestionService : IAudioStreamIngestionService
{
    public AudioChunkIngestionResponse? SuccessResponse { get; set; }
    public bool ThrowInvalidChunk { get; set; }

    public Task<AudioChunkIngestionResponse> IngestChunkAsync(AudioChunkIngestionRequest request, string correlationId, CancellationToken cancellationToken = default)
    {
        if (ThrowInvalidChunk)
        {
            throw new InvalidAudioChunkException("invalid");
        }

        return Task.FromResult(SuccessResponse ?? new AudioChunkIngestionResponse
        {
            Accepted = true,
            CallId = request.CallId,
            Sequence = request.Sequence,
            CorrelationId = correlationId
        });
    }
}
