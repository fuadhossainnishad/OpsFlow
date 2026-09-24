using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Teams;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");

        builder.HasKey(member => member.Id);

        builder.Property(member => member.AddedAtUtc)
            .IsRequired();

        builder.HasIndex(member => new
        {
            member.TeamId,
            member.MembershipId
        })
        .IsUnique()
        .HasDatabaseName("UX_TeamMembers_Team_Membership");

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(member => member.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<OpsFlow.Domain.Organizations.Membership>()
            .WithMany()
            .HasForeignKey(member => member.MembershipId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
