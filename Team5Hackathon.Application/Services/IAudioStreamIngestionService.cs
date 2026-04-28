using Team5Hackathon.Application.DTOs.Streaming;

namespace Team5Hackathon.Application.Services;

public interface IAudioStreamIngestionService
{
    Task<AudioChunkIngestionResponse> IngestChunkAsync(
        AudioChunkIngestionRequest request,
        string correlationId,
        CancellationToken cancellationToken = default);
}
