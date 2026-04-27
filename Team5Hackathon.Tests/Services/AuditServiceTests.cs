using Team5Hackathon.Application.DTOs.Audit;
using Team5Hackathon.Application.Services;
using Team5Hackathon.Tests.Fakes;

namespace Team5Hackathon.Tests.Services;

public sealed class AuditServiceTests
{
    [Fact]
    public async Task RecordAsync_MapsAndPersistsEntry()
    {
        var repository = new InMemoryAuditLogRepository();
        var service = new AuditService(repository);
        var entry = new AuditEntry
        {
            Action = "users.create",
            Outcome = "success",
            EntityName = "User",
            EntityId = "123",
            Description = "New user created",
            CorrelationId = "corr-1",
            PerformedBy = "admin@example.com",
            PerformedById = "admin-1",
            IpAddress = "127.0.0.1",
            UserAgent = "xunit"
        };

        await service.RecordAsync(entry);

        var stored = Assert.Single(repository.Stored);
        Assert.Equal(entry.Action, stored.Action);
        Assert.Equal(entry.Outcome, stored.Outcome);
        Assert.Equal(entry.EntityName, stored.EntityName);
        Assert.Equal(entry.EntityId, stored.EntityId);
        Assert.Equal(entry.CorrelationId, stored.CorrelationId);
        Assert.Equal(entry.PerformedBy, stored.PerformedBy);
        Assert.Equal(entry.PerformedById, stored.PerformedById);
        Assert.Equal(entry.IpAddress, stored.IpAddress);
        Assert.Equal(entry.UserAgent, stored.UserAgent);
    }
}
