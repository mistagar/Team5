using Microsoft.AspNetCore.Mvc;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs.Streaming;
using Team5Hackathon.Application.Exceptions;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.API.Controllers;

[ApiController]
[Route("api/stream")]
public sealed class AudioStreamController : ControllerBase
{
    private readonly IAudioStreamIngestionService _audioStreamIngestionService;

    public AudioStreamController(IAudioStreamIngestionService audioStreamIngestionService)
    {
        _audioStreamIngestionService = audioStreamIngestionService;
    }

    [HttpPost("audio-chunk")]
    public async Task<IActionResult> IngestAudioChunk(
        [FromBody] AudioChunkIngestionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = HttpContext.TraceIdentifier;
            var response = await _audioStreamIngestionService.IngestChunkAsync(
                request,
                correlationId,
                cancellationToken);

            return Accepted(ApiResponse<AudioChunkIngestionResponse>.SuccessResponse(response));
        }
        catch (InvalidAudioChunkException ex)
        {
            return BadRequest(ApiResponse<AudioChunkIngestionResponse>.FailResponse(ex.Message));
        }
    }
}
