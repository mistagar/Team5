using Microsoft.EntityFrameworkCore;
using Team5Hackathon.Domain.Entities;

namespace Team5Hackathon.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var auditLog = modelBuilder.Entity<AuditLog>();
        auditLog.ToTable("AuditLogs");
        auditLog.HasKey(x => x.Id);
        auditLog.Property(x => x.Action).HasMaxLength(120).IsRequired();
        auditLog.Property(x => x.Outcome).HasMaxLength(50).IsRequired();
        auditLog.Property(x => x.EntityName).HasMaxLength(100);
        auditLog.Property(x => x.EntityId).HasMaxLength(100);
        auditLog.Property(x => x.Description).HasMaxLength(1000);
        auditLog.Property(x => x.CorrelationId).HasMaxLength(100);
        auditLog.Property(x => x.PerformedBy).HasMaxLength(200);
        auditLog.Property(x => x.PerformedById).HasMaxLength(120);
        auditLog.Property(x => x.IpAddress).HasMaxLength(120);
        auditLog.Property(x => x.UserAgent).HasMaxLength(500);
        auditLog.Property(x => x.CreatedAtUtc).IsRequired();
        auditLog.HasIndex(x => x.CreatedAtUtc);
        auditLog.HasIndex(x => x.CorrelationId);
    }
}
