using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Team5Hackathon.API.Controllers;
using Team5Hackathon.API.DTOs;
using Team5Hackathon.Application.DTOs.Streaming;
using Team5Hackathon.Tests.Fakes;

namespace Team5Hackathon.Tests.Controllers;

public sealed class AudioStreamControllerTests
{
    [Fact]
    public async Task IngestAudioChunk_ReturnsAccepted_ForValidRequest()
    {
        var service = new StubAudioStreamIngestionService();
        var controller = new AudioStreamController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { TraceIdentifier = "corr-test" }
            }
        };

        var result = await controller.IngestAudioChunk(new AudioChunkIngestionRequest
        {
            CallId = Guid.NewGuid(),
            Sequence = 1,
            ChunkBase64 = Convert.ToBase64String(new byte[] { 1 }),
            SentAtUtc = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var accepted = Assert.IsType<AcceptedResult>(result);
        var payload = Assert.IsType<ApiResponse<AudioChunkIngestionResponse>>(accepted.Value);
        Assert.True(payload.Success);
        Assert.True(payload.Data?.Accepted);
    }

    [Fact]
    public async Task IngestAudioChunk_ReturnsBadRequest_ForInvalidChunk()
    {
        var service = new StubAudioStreamIngestionService { ThrowInvalidChunk = true };
        var controller = new AudioStreamController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { TraceIdentifier = "corr-test" }
            }
        };

        var result = await controller.IngestAudioChunk(new AudioChunkIngestionRequest
        {
            CallId = Guid.NewGuid(),
            Sequence = 1,
            ChunkBase64 = "invalid",
            SentAtUtc = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var payload = Assert.IsType<ApiResponse<AudioChunkIngestionResponse>>(badRequest.Value);
        Assert.False(payload.Success);
    }
}
