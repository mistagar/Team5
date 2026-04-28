using Team5Hackathon.Application.DTOs.Streaming;
using Team5Hackathon.Application.Exceptions;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Tests.Fakes;

namespace Team5Hackathon.Tests.Services;

public sealed class AudioStreamIngestionServiceTests
{
    [Fact]
    public async Task IngestChunkAsync_AcceptsValidChunk_AndQueuesIt()
    {
        var queue = new RecordingAudioStreamQueue();
        var audit = new RecordingAuditService();
        var service = new AudioStreamIngestionService(queue, audit);
        var callId = Guid.NewGuid();
        var request = new AudioChunkIngestionRequest
        {
            CallId = callId,
            Sequence = 1,
            ChunkBase64 = Convert.ToBase64String(new byte[] { 1, 2, 3 }),
            SentAtUtc = DateTimeOffset.UtcNow
        };

        var response = await service.IngestChunkAsync(request, "corr-123");

        Assert.True(response.Accepted);
        Assert.Equal(callId, response.CallId);
        Assert.Equal(1, response.Sequence);
        Assert.Single(queue.Queued);
        Assert.Empty(audit.Entries);
    }

    [Fact]
    public async Task IngestChunkAsync_RejectsInvalidBase64_AndAuditsFailure()
    {
        var queue = new RecordingAudioStreamQueue();
        var audit = new RecordingAuditService();
        var service = new AudioStreamIngestionService(queue, audit);
        var request = new AudioChunkIngestionRequest
        {
            CallId = Guid.NewGuid(),
            Sequence = 1,
            ChunkBase64 = "not-base64",
            SentAtUtc = DateTimeOffset.UtcNow
        };

        await Assert.ThrowsAsync<InvalidAudioChunkException>(() => service.IngestChunkAsync(request, "corr-124"));

        Assert.Empty(queue.Queued);
        Assert.Single(audit.Entries);
        Assert.Equal("failed", audit.Entries[0].Outcome);
    }

    [Fact]
    public async Task IngestChunkAsync_RejectsOutOfOrderSequence()
    {
        var queue = new RecordingAudioStreamQueue();
        var audit = new RecordingAuditService();
        var service = new AudioStreamIngestionService(queue, audit);
        var callId = Guid.NewGuid();
        var chunk = Convert.ToBase64String(new byte[] { 9, 9, 9 });

        await service.IngestChunkAsync(new AudioChunkIngestionRequest
        {
            CallId = callId,
            Sequence = 2,
            ChunkBase64 = chunk,
            SentAtUtc = DateTimeOffset.UtcNow
        }, "corr-125");

        await Assert.ThrowsAsync<InvalidAudioChunkException>(() => service.IngestChunkAsync(new AudioChunkIngestionRequest
        {
            CallId = callId,
            Sequence = 2,
            ChunkBase64 = chunk,
            SentAtUtc = DateTimeOffset.UtcNow
        }, "corr-126"));
    }
}
