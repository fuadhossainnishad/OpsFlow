using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class ApprovalRequestConfiguration
    : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.ToTable("ApprovalRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.RequesterUserId).IsRequired();
        builder.Property(x => x.TimeEntryId).IsRequired();

        builder.Property(x => x.Comment).HasMaxLength(2000);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DecidedByUserId);
        builder.Property(x => x.DecisionComment).HasMaxLength(2000);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.DecidedAtUtc);

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.Status,
            x.CreatedAtUtc
        });

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.TimeEntryId
        })
        .IsUnique()
        .HasFilter("[Status] = 1");

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.RequesterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.DecidedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.TimeTracking.TimeEntry>()
            .WithMany()
            .HasForeignKey(x => x.TimeEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
