using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class RoleConfiguration
    : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.NormalizedName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.IsSystemRole)
            .IsRequired();

        builder.HasIndex(role => role.NormalizedName)
            .IsUnique()
            .HasDatabaseName("UX_Roles_NormalizedName");

        builder.HasData(
new
{
    Id = SystemRoles.OwnerId,
    Name = "Owner",
    NormalizedName = "OWNER",
    IsSystemRole = true
},
new
{
    Id = SystemRoles.AdminId,
    Name = "Admin",
    NormalizedName = "ADMIN",
    IsSystemRole = true
},
new
{
    Id = SystemRoles.ProjectManagerId,
    Name = "Project Manager",
    NormalizedName = "PROJECT_MANAGER",
    IsSystemRole = true
},
new
{
    Id = SystemRoles.TeamLeadId,
    Name = "Team Lead",
    NormalizedName = "TEAM_LEAD",
    IsSystemRole = true
},
new
{
    Id = SystemRoles.MemberId,
    Name = "Member",
    NormalizedName = "MEMBER",
    IsSystemRole = true
},
new
{
    Id = SystemRoles.ViewerId,
    Name = "Viewer",
    NormalizedName = "VIEWER",
    IsSystemRole = true
});
    }

}
