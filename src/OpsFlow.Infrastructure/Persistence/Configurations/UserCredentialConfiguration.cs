using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class UserCredentialConfiguration
    : IEntityTypeConfiguration<UserCredential>
{
    public void Configure(EntityTypeBuilder<UserCredential> builder)
    {
        builder.ToTable("UserCredentials");

        builder.HasKey(credential => credential.Id);

        builder.Property(credential => credential.Id)
            .ValueGeneratedNever();

        builder.Property(credential => credential.UserId)
            .IsRequired();

        builder.Property(credential => credential.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(credential => credential.PasswordChangedAtUtc)
            .IsRequired();

        builder.HasIndex(credential => credential.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<UserCredential>(
                credential => credential.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
