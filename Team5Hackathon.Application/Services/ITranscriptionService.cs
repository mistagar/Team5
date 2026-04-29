namespace Team5Hackathon.Application.Services;

public interface ITranscriptionService
{
    Task<string?> TranscribeAsync(
        byte[] audioBytes,
        string fileName,
        string? contentType = null,
        CancellationToken cancellationToken = default);
}
