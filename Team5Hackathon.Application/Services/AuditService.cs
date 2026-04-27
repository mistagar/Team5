using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Application.Services;

public sealed class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Action = entry.Action,
            Outcome = entry.Outcome,
            EntityName = entry.EntityName,
            EntityId = entry.EntityId,
            Description = entry.Description,
            CorrelationId = entry.CorrelationId,
            PerformedBy = entry.PerformedBy,
            PerformedById = entry.PerformedById,
            IpAddress = entry.IpAddress,
            UserAgent = entry.UserAgent
        };

        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
    }
}
