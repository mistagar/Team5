using Team5Hackathon.Application.DTOs.Audit;

namespace Team5Hackathon.Application.Services;

public interface IAuditService
{
    Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
