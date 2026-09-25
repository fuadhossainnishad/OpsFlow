using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.TimeTracking;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class TimeEntryConfiguration
    : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.OrganizationId).IsRequired();
        builder.Property(entry => entry.UserId).IsRequired();
        builder.Property(entry => entry.ProjectId).IsRequired();
        builder.Property(entry => entry.TaskId);

        builder.Property(entry => entry.Description)
            .HasMaxLength(1000);

        builder.Property(entry => entry.StartedAtUtc)
            .IsRequired();

        builder.Property(entry => entry.EndedAtUtc);

        builder.Property(entry => entry.DurationSeconds);

        builder.Property(entry => entry.IsManual)
            .IsRequired();

        builder.Property(entry => entry.CreatedAtUtc)
            .IsRequired();

        builder.Property(entry => entry.UpdatedAtUtc);

        builder.HasIndex(entry => new
        {
            entry.OrganizationId,
            entry.UserId,
            entry.StartedAtUtc
        })
        .HasDatabaseName("IX_TimeEntries_Organization_User_Started");

        builder.HasIndex(entry => new
        {
            entry.OrganizationId,
            entry.ProjectId,
            entry.StartedAtUtc
        })
        .HasDatabaseName("IX_TimeEntries_Organization_Project_Started");

        builder.HasIndex(entry => new
        {
            entry.OrganizationId,
            entry.TaskId,
            entry.StartedAtUtc
        })
        .HasDatabaseName("IX_TimeEntries_Organization_Task_Started");

        builder.HasIndex(entry => new
        {
            entry.OrganizationId,
            entry.UserId
        })
        .IsUnique()
        .HasFilter("[EndedAtUtc] IS NULL")
        .HasDatabaseName("UX_TimeEntries_Organization_User_Running");

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(entry => entry.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(entry => entry.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Projects.Project>()
            .WithMany()
            .HasForeignKey(entry => entry.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Tasks.TaskItem>()
            .WithMany()
            .HasForeignKey(entry => entry.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
