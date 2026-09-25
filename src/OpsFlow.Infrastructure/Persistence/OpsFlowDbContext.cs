using Microsoft.EntityFrameworkCore;
using OpsFlow.Domain.Auditing;
using OpsFlow.Domain.Authorization;
using OpsFlow.Domain.Identity;
using OpsFlow.Domain.Organizations;
using OpsFlow.Domain.Projects;
using OpsFlow.Domain.Tasks;
using OpsFlow.Domain.Teams;
namespace OpsFlow.Infrastructure.Persistence;

public sealed class OpsFlowDbContext(
    DbContextOptions<OpsFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<Membership> Memberships => Set<Membership>();

    public DbSet<OrganizationInvitation> OrganizationInvitations => Set<OrganizationInvitation>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<UserCredential> UserCredentials => Set<UserCredential>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OpsFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

