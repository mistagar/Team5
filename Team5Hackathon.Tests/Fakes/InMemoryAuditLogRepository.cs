using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Domain.RepositoriesContract;

namespace Team5Hackathon.Tests.Fakes;

internal sealed class InMemoryAuditLogRepository : IAuditLogRepository
{
    public List<AuditLog> Stored { get; } = new();

    public Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        Stored.Add(auditLog);
        return Task.CompletedTask;
    }
}
