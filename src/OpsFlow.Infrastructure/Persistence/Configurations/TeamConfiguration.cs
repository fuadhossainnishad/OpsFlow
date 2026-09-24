using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Teams;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(team => team.Id);

        builder.Property(team => team.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(team => team.NormalizedName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(team => team.Description)
            .HasMaxLength(1000);

        builder.Property(team => team.TeamLeadMembershipId);

        builder.Property(team => team.IsArchived)
            .IsRequired();

        builder.Property(team => team.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(team => new
        {
            team.OrganizationId,
            team.NormalizedName
        })
        .IsUnique()
        .HasDatabaseName("UX_Teams_Organization_NormalizedName");

        builder.HasIndex(team => new
        {
            team.OrganizationId,
            team.IsArchived
        })
        .HasDatabaseName("IX_Teams_Organization_Archived");

        builder.HasOne<OpsFlow.Domain.Organizations.Organization>()
            .WithMany()
            .HasForeignKey(team => team.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<OpsFlow.Domain.Organizations.Membership>()
            .WithMany()
            .HasForeignKey(team => team.TeamLeadMembershipId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
