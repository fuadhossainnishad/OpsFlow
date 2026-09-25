using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Projects;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Key)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(2000);

        builder.Property(project => project.OrganizationId)
            .IsRequired();

        builder.Property(project => project.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(project => project.CreatedAtUtc)
            .IsRequired();

        builder.Property(project => project.ArchivedAtUtc);

        builder.HasIndex(project => new
        {
            project.OrganizationId,
            project.Key
        })
        .IsUnique();

        builder.HasIndex(project => new
        {
            project.OrganizationId,
            project.Status
        });
    }
}
