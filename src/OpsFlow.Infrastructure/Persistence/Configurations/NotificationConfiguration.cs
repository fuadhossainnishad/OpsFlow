using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Notifications;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.Message)
            .HasMaxLength(2000);

        builder.Property(x => x.ResourceType)
            .HasMaxLength(100);

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.RecipientUserId,
            x.IsRead,
            x.CreatedAtUtc
        })
        .HasDatabaseName("IX_Notifications_Recipient_Read_Created");

        builder.HasIndex(x => new
        {
            x.OrganizationId,
            x.RecipientUserId,
            x.CreatedAtUtc
        })
        .HasDatabaseName("IX_Notifications_Recipient_Created");

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Identity.User>()
            .WithMany()
            .HasForeignKey(x => x.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
