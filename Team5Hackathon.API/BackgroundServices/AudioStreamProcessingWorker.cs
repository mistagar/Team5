using Team5Hackathon.Application.Constants;
using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Application.Streaming;

namespace Team5Hackathon.API.BackgroundServices;

public sealed class AudioStreamProcessingWorker : BackgroundService
{
    private readonly IAudioStreamQueue _audioStreamQueue;
    private readonly InMemoryAudioChunkBuffer _buffer;
    private readonly ILogger<AudioStreamProcessingWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public AudioStreamProcessingWorker(
        IAudioStreamQueue audioStreamQueue,
        InMemoryAudioChunkBuffer buffer,
        ILogger<AudioStreamProcessingWorker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _audioStreamQueue = audioStreamQueue;
        _buffer = buffer;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var chunk in _audioStreamQueue.ReadAllAsync(stoppingToken))
        {
            try
            {
                _buffer.Add(chunk);
                _logger.LogInformation(
                    "Recognition progress for CallId {CallId} at Sequence {Sequence}.",
                    chunk.CallId,
                    chunk.Sequence);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Audio stream processing failed for CallId {CallId}, Sequence {Sequence}.",
                    chunk.CallId,
                    chunk.Sequence);

                using var scope = _scopeFactory.CreateScope();
                var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
                await auditService.RecordAsync(
                new AuditEntry
                {
                    Action = AuditActionNames.StreamAudioChunkProcess,
                    Outcome = "failed",
                    EntityName = "Call",
                    EntityId = chunk.CallId.ToString(),
                    Description = "Failed to process queued audio chunk.",
                    CorrelationId = chunk.CorrelationId
                },
                stoppingToken);
            }
        }
    }
}
