using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Authorization;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class OrganizationInvitationConfiguration
    : IEntityTypeConfiguration<OrganizationInvitation>
{
    public void Configure(EntityTypeBuilder<OrganizationInvitation> builder)
    {
        builder.ToTable("OrganizationInvitations");

        builder.HasKey(invitation => invitation.Id);

        builder.Property(invitation => invitation.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(invitation => invitation.NormalizedEmail)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(invitation => invitation.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(invitation => invitation.ExpiresAtUtc)
            .IsRequired();

        builder.HasIndex(invitation => invitation.TokenHash)
            .IsUnique()
            .HasDatabaseName("UX_OrganizationInvitations_TokenHash");

        builder.HasIndex(invitation => new
        {
            invitation.OrganizationId,
            invitation.NormalizedEmail
        })
        .HasDatabaseName("IX_OrganizationInvitations_Organization_Email");

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(invitation => invitation.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(invitation => invitation.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
