using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Application.Services;

namespace Team5Hackathon.Tests.Fakes;

internal sealed class RecordingAuditService : IAuditService
{
    public List<AuditEntry> Entries { get; } = new();

    public Task RecordAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}
