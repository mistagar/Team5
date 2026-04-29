using Team5Hackathon.Application.DTOs.AI;

namespace Team5Hackathon.Application.Services;

public interface ITextToSpeechService
{
    /// <summary>
    /// Synthesises <paramref name="text"/> to MP3 audio using Azure Cognitive Services.
    /// Returns a base-64 encoded string of the resulting audio bytes.
    /// </summary>
    Task<TextToSpeechResponse> SynthesiseAsync(
        TextToSpeechRequest request,
        CancellationToken cancellationToken = default);
}
