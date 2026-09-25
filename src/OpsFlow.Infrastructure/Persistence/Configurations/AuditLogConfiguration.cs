using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Auditing;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(audit => audit.Id);

        builder.Property(audit => audit.OrganizationId)
            .IsRequired();

        builder.Property(audit => audit.ActorUserId);

        builder.Property(audit => audit.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(audit => audit.Resource)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(audit => audit.ResourceId);

        builder.Property(audit => audit.OccurredAtUtc)
            .IsRequired();

        builder.Property(audit => audit.IpAddress)
            .HasMaxLength(64);

        builder.Property(audit => audit.UserAgent)
            .HasMaxLength(1000);

        builder.Property(audit => audit.CorrelationId)
            .HasMaxLength(128);

        builder.Property(audit => audit.BeforeJson)
            .HasMaxLength(10000);

        builder.Property(audit => audit.AfterJson)
            .HasMaxLength(10000);

        builder.HasIndex(audit => new
        {
            audit.OrganizationId,
            audit.OccurredAtUtc
        });

        builder.HasIndex(audit => new
        {
            audit.OrganizationId,
            audit.Resource,
            audit.ResourceId
        });

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(audit => audit.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(audit => audit.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
