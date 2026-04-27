using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Domain.RepositoriesContract;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
}
