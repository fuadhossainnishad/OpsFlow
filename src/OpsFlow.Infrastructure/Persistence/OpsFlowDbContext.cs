using Microsoft.EntityFrameworkCore;
using OpsFlow.Domain.Authorization;
using OpsFlow.Domain.Identity;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Infrastructure.Persistence;

public sealed class OpsFlowDbContext(
    DbContextOptions<OpsFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<Membership> Memberships => Set<Membership>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OpsFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

