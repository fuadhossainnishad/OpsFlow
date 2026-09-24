using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class TaskItemConfiguration
    : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.OrganizationId)
            .IsRequired();

        builder.Property(task => task.ProjectId)
            .IsRequired();

        builder.Property(task => task.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasMaxLength(5000);

        builder.Property(task => task.AssigneeUserId);

        builder.Property(task => task.Status)
            .IsRequired();

        builder.Property(task => task.CreatedAtUtc)
            .IsRequired();

        builder.Property(task => task.UpdatedAtUtc);

        builder.Property(task => task.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasIndex(task => new
        {
            task.OrganizationId,
            task.ProjectId
        })
        .HasDatabaseName("IX_Tasks_Organization_Project");

        builder.HasIndex(task => new
        {
            task.OrganizationId,
            task.AssigneeUserId
        })
        .HasDatabaseName("IX_Tasks_Organization_Assignee");

        builder.HasIndex(task => new
        {
            task.OrganizationId,
            task.Status
        })
        .HasDatabaseName("IX_Tasks_Organization_Status");

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(task => task.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Projects.Project>()
            .WithMany()
            .HasForeignKey(task => task.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(task => task.AssigneeUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
