using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Team5Hackathon.Domain.Entities;
using Team5Hackathon.Infrastructure.Identity;

namespace Team5Hackathon.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Call> Calls { get; set; } = null!;
        public DbSet<TranscriptSegment> TranscriptSegments { get; set; } = null!;
        public DbSet<FollowUpMessage> FollowUpMessages { get; set; } = null!;
        public DbSet<UserRequest> UserRequests { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne<ApplicationUser>()
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Rename Identity tables here:
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");


            var auditLog = builder.Entity<AuditLog>();
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

            builder.Entity<Call>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Transcript).HasMaxLength(10000);
                entity.Property(c => c.Summary).HasMaxLength(2000);
                entity.Property(c => c.ActionItems).HasMaxLength(2000);
                entity.Property(c => c.PrimaryIntent).HasMaxLength(100);
                entity.Property(c => c.Status).HasMaxLength(50);
                entity.Property(c => c.Category).HasMaxLength(50);
                entity.Property(c => c.Sentiment).HasMaxLength(50);
            });

            builder.Entity<TranscriptSegment>(entity =>
            {
                entity.HasKey(ts => ts.Id);
                entity.HasOne<Call>().WithMany().HasForeignKey(ts => ts.CallId);
                entity.Property(ts => ts.Text).HasMaxLength(1000);
                entity.Property(ts => ts.Intent).HasMaxLength(100);
                entity.Property(ts => ts.Entities).HasMaxLength(1000);
            });

            builder.Entity<FollowUpMessage>(entity =>
            {
                entity.HasKey(fm => fm.Id);
                entity.HasOne<Call>().WithMany().HasForeignKey(fm => fm.CallId);
                entity.Property(fm => fm.Type).HasMaxLength(20);
                entity.Property(fm => fm.Content).HasMaxLength(2000);
                entity.Property(fm => fm.DeliveryStatus).HasMaxLength(50);
            });

            builder.Entity<UserRequest>(entity =>
            {
                entity.HasKey(ur => ur.Id);
                entity.HasOne<User>().WithMany(u => u.Requests).HasForeignKey(ur => ur.UserId);
                entity.Property(ur => ur.Type).HasMaxLength(50);
                entity.Property(ur => ur.Content).HasMaxLength(5000);
                entity.Property(ur => ur.Status).HasMaxLength(50);
                entity.Property(ur => ur.Summary).HasMaxLength(2000);
                entity.Property(ur => ur.Response).HasMaxLength(5000);
                entity.Property(ur => ur.Category).HasMaxLength(50);
                entity.Property(ur => ur.Sentiment).HasMaxLength(50);
                // CallId is optional — null for non-call requests
                entity.HasIndex(ur => ur.CallId);
            });

            var adminRoleId = Guid.Parse("c4a3298c-6198-4d12-bd1a-56d1d1ce0aa7");
            var supervisorRoleId = Guid.Parse("582880c3-f554-490f-a24e-526db35cffa5");
            var customerRoleId = Guid.Parse("a3d7f9b1-8c42-4e6d-b5a9-91c2e7f4d8ab");
            var agentRoleId = Guid.Parse("e8b9c5d2-3f1a-4b7e-9d6c-2a8f7e5b4c3d");

            builder.Entity<ApplicationRole>().HasData(
               new ApplicationRole
               {
                   Id = adminRoleId,
                   Name = "Admin",
                   NormalizedName = "ADMIN",
                   Description = "Administrator with full permissions"
               },
               new ApplicationRole
               {
                   Id = supervisorRoleId,
                   Name = "Supervisor",
                   NormalizedName = "SUPERVISOR",
                   Description = "Supervisor with supervisory permissions"
               },
               new ApplicationRole
               {
                   Id = customerRoleId,
                   Name = "Customer",
                   NormalizedName = "CUSTOMER",
                   Description = "Customer with Customer access"
               },
                new ApplicationRole
                {
                    Id = agentRoleId,
                    Name = "Agent",
                    NormalizedName = "AGENT",
                    Description = "Agent with agent permissions"
                }
           );
            builder.Entity<Client>().HasData(
                new Client { ClientId = "web", ClientName = "Web Client", Description = "Web browser clients", IsActive = true },
                new Client { ClientId = "android", ClientName = "Android Client", Description = "Android mobile app", IsActive = true },
                new Client { ClientId = "ios", ClientName = "iOS Client", Description = "iOS mobile app", IsActive = true }
            );
        }
    }

}

