using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Organizations;
using OpsFlow.Domain.Authorization;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class MembershipConfiguration
    : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("Memberships");

        builder.HasKey(membership => membership.Id);

        builder.Property(membership => membership.IsActive)
            .IsRequired();

        builder.Property(membership => membership.JoinedAtUtc)
            .IsRequired();

        builder.HasIndex(membership => new
        {
            membership.OrganizationId,
            membership.UserId
        })
        .IsUnique()
        .HasDatabaseName("UX_Memberships_Organization_User");

        builder.HasIndex(membership => new
        {
            membership.OrganizationId,
            membership.IsActive
        })
        .HasDatabaseName("IX_Memberships_Organization_Active");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(membership => membership.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(membership => membership.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
