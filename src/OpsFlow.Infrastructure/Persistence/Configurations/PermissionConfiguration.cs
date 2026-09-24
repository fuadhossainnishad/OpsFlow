using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class PermissionConfiguration
    : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Code)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(permission => permission.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(permission => permission.Code)
            .IsUnique()
            .HasDatabaseName("UX_Permissions_Code");

        builder.HasData(
            new
            {
                Id = PermissionIds.OrganizationsRead,
                Code = "organizations.read",
                Name = "Read organizations"
            },
            new
            {
                Id = PermissionIds.OrganizationsUpdate,
                Code = "organizations.update",
                Name = "Update organizations"
            },
            new
            {
                Id = PermissionIds.MembersRead,
                Code = "members.read",
                Name = "Read members"
            },
            new
            {
                Id = PermissionIds.MembersInvite,
                Code = "members.invite",
                Name = "Invite members"
            },
            new
            {
                Id = PermissionIds.MembersManage,
                Code = "members.manage",
                Name = "Manage members"
            },
            new
            {
                Id = PermissionIds.ProjectsRead,
                Code = "projects.read",
                Name = "Read projects"
            },
            new
            {
                Id = PermissionIds.ProjectsCreate,
                Code = "projects.create",
                Name = "Create projects"
            },
            new
            {
                Id = PermissionIds.ProjectsUpdate,
                Code = "projects.update",
                Name = "Update projects"
            },
            new
            {
                Id = PermissionIds.TasksRead,
                Code = "tasks.read",
                Name = "Read tasks"
            },
            new
            {
                Id = PermissionIds.TasksCreate,
                Code = "tasks.create",
                Name = "Create tasks"
            },
            new
            {
                Id = PermissionIds.TasksUpdate,
                Code = "tasks.update",
                Name = "Update tasks"
            },
            new
            {
                Id = PermissionIds.TasksAssign,
                Code = "tasks.assign",
                Name = "Assign tasks"
            },
            new
            {
                Id = PermissionIds.ReportsRead,
                Code = "reports.read",
                Name = "Read reports"
            },
            new
            {
                Id = PermissionIds.BillingManage,
                Code = "billing.manage",
                Name = "Manage billing"
            });
    }
}
