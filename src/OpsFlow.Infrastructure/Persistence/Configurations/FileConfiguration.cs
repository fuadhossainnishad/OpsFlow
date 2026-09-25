using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Files;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class FileConfiguration : IEntityTypeConfiguration<FileRecord>
{
    public void Configure(EntityTypeBuilder<FileRecord> builder)
    {
        builder.ToTable("Files");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.StorageKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => x.StorageKey)
            .IsUnique();

        builder.Property(x => x.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SizeBytes)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.CreatedAtUtc
        });

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
