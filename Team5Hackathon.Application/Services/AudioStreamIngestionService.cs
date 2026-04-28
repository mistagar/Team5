using System.Collections.Concurrent;
using Team5Hackathon.Application.Constants;
using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Application.DTOs.Streaming;
using Team5Hackathon.Application.Exceptions;
using Team5Hackathon.Application.Streaming;

namespace Team5Hackathon.Application.Services;

public sealed class AudioStreamIngestionService : IAudioStreamIngestionService
{
    private const int MaxChunkSizeBytes = 262_144;
    private readonly ConcurrentDictionary<Guid, long> _lastSequenceByCall = new();
    private readonly IAudioStreamQueue _audioStreamQueue;
    private readonly IAuditService _auditService;

    public AudioStreamIngestionService(IAudioStreamQueue audioStreamQueue, IAuditService auditService)
    {
        _audioStreamQueue = audioStreamQueue;
        _auditService = auditService;
    }

    public async Task<AudioChunkIngestionResponse> IngestChunkAsync(
        AudioChunkIngestionRequest request,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRequest(request);

            var chunkBytes = DecodeChunk(request.ChunkBase64);
            ValidateChunkSize(chunkBytes.Length);
            ValidateSequenceOrdering(request.CallId, request.Sequence);

            await _audioStreamQueue.QueueAsync(
                new AudioChunkEnvelope
                {
                    CallId = request.CallId,
                    Sequence = request.Sequence,
                    ChunkBytes = chunkBytes,
                    SentAtUtc = request.SentAtUtc,
                    CorrelationId = correlationId
                },
                cancellationToken);

            return new AudioChunkIngestionResponse
            {
                CallId = request.CallId,
                Sequence = request.Sequence,
                Accepted = true,
                CorrelationId = correlationId
            };
        }
        catch (InvalidAudioChunkException ex)
        {
            await _auditService.RecordAsync(
                new AuditEntry
                {
                    Action = AuditActionNames.StreamAudioChunkIngest,
                    Outcome = "failed",
                    EntityName = "Call",
                    EntityId = request.CallId == Guid.Empty ? null : request.CallId.ToString(),
                    Description = ex.Message,
                    CorrelationId = correlationId
                },
                cancellationToken);

            throw;
        }
    }

    private static void ValidateRequest(AudioChunkIngestionRequest request)
    {
        if (request.CallId == Guid.Empty)
        {
            throw new InvalidAudioChunkException("CallId is required.");
        }

        if (request.Sequence <= 0)
        {
            throw new InvalidAudioChunkException("Sequence must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.ChunkBase64))
        {
            throw new InvalidAudioChunkException("ChunkBase64 is required.");
        }
    }

    private static byte[] DecodeChunk(string chunkBase64)
    {
        try
        {
            return Convert.FromBase64String(chunkBase64);
        }
        catch (FormatException)
        {
            throw new InvalidAudioChunkException("ChunkBase64 is not a valid Base64 value.");
        }
    }

    private static void ValidateChunkSize(int sizeBytes)
    {
        if (sizeBytes == 0)
        {
            throw new InvalidAudioChunkException("Decoded audio chunk cannot be empty.");
        }

        if (sizeBytes > MaxChunkSizeBytes)
        {
            throw new InvalidAudioChunkException("Decoded audio chunk exceeds the maximum allowed size.");
        }
    }

    private void ValidateSequenceOrdering(Guid callId, long sequence)
    {
        var currentLast = _lastSequenceByCall.AddOrUpdate(callId, sequence, (_, existing) =>
        {
            if (sequence <= existing)
            {
                throw new InvalidAudioChunkException("Sequence must be strictly increasing per call.");
            }

            return sequence;
        });

        if (currentLast != sequence)
        {
            throw new InvalidAudioChunkException("Invalid sequence state.");
        }
    }
}
