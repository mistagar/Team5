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
    private readonly CallRecordingService _callRecordingService;

    public AudioStreamController(IAudioStreamIngestionService audioStreamIngestionService, CallRecordingService callRecordingService)
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
            
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
    }
}
