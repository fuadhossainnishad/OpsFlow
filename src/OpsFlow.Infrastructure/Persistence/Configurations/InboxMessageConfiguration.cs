using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Messaging;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class InboxMessageConfiguration
    : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(message => message.Id);

        builder.Property(message => message.MessageType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.ReceivedAt)
            .IsRequired();

        builder.Property(message => message.ProcessedAt)
            .IsRequired();

        builder.HasIndex(message => message.ProcessedAt);
    }
}
