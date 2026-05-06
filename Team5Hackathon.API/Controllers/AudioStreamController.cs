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
    private readonly ICallRecordingService _callRecordingService;

    public AudioStreamController(IAudioStreamIngestionService audioStreamIngestionService, ICallRecordingService callRecordingService)
    {
        _audioStreamIngestionService = audioStreamIngestionService;
        _callRecordingService = callRecordingService;
    }

    [HttpPost("audio-chunk")]
    public async Task<IActionResult> IngestAudioChunk(
        [FromBody] AudioChunkIngestionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var correlationId = HttpContext.TraceIdentifier;

            // Use CancellationToken.None for the queue write: the request body is already
            // fully read at this point, so cancelling here would silently drop a valid chunk.
            var response = await _audioStreamIngestionService.IngestChunkAsync(
                request,
                correlationId,
                CancellationToken.None);

            if (!string.IsNullOrEmpty(request.ChunkBase64))
            {
                var chunkBytes = Convert.FromBase64String(request.ChunkBase64);
                _callRecordingService.AddAudioChunk(request.CallId, chunkBytes);
            }

            return Accepted(ApiResponse<AudioChunkIngestionResponse>.SuccessResponse(response));
        }
        catch (InvalidAudioChunkException ex)
        {
            return BadRequest(ApiResponse<AudioChunkIngestionResponse>.FailResponse(ex.Message));
        }
        catch (OperationCanceledException)
        {
            // Client disconnected before the body was fully read — not an error.
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
    }
}
