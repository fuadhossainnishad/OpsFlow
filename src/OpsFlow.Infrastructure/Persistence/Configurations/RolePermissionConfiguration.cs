using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration
    : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.HasKey(rolePermission => new
        {
            rolePermission.RoleId,
            rolePermission.PermissionId
        });

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            Owner(PermissionIds.OrganizationsRead),
            Owner(PermissionIds.OrganizationsUpdate),
            Owner(PermissionIds.MembersRead),
            Owner(PermissionIds.MembersInvite),
            Owner(PermissionIds.MembersManage),
            Owner(PermissionIds.ProjectsRead),
            Owner(PermissionIds.ProjectsCreate),
            Owner(PermissionIds.ProjectsUpdate),
            Owner(PermissionIds.TasksRead),
            Owner(PermissionIds.TasksCreate),
            Owner(PermissionIds.TasksUpdate),
            Owner(PermissionIds.TasksAssign),
            Owner(PermissionIds.ReportsRead),
            Owner(PermissionIds.BillingManage),

            Admin(PermissionIds.OrganizationsRead),
            Admin(PermissionIds.OrganizationsUpdate),
            Admin(PermissionIds.MembersRead),
            Admin(PermissionIds.MembersInvite),
            Admin(PermissionIds.MembersManage),
            Admin(PermissionIds.ProjectsRead),
            Admin(PermissionIds.ProjectsCreate),
            Admin(PermissionIds.ProjectsUpdate),
            Admin(PermissionIds.TasksRead),
            Admin(PermissionIds.TasksCreate),
            Admin(PermissionIds.TasksUpdate),
            Admin(PermissionIds.TasksAssign),
            Admin(PermissionIds.ReportsRead),

            ProjectManager(PermissionIds.OrganizationsRead),
            ProjectManager(PermissionIds.MembersRead),
            ProjectManager(PermissionIds.ProjectsRead),
            ProjectManager(PermissionIds.ProjectsCreate),
            ProjectManager(PermissionIds.ProjectsUpdate),
            ProjectManager(PermissionIds.TasksRead),
            ProjectManager(PermissionIds.TasksCreate),
            ProjectManager(PermissionIds.TasksUpdate),
            ProjectManager(PermissionIds.TasksAssign),
            ProjectManager(PermissionIds.ReportsRead),

            TeamLead(PermissionIds.OrganizationsRead),
            TeamLead(PermissionIds.MembersRead),
            TeamLead(PermissionIds.ProjectsRead),
            TeamLead(PermissionIds.TasksRead),
            TeamLead(PermissionIds.TasksCreate),
            TeamLead(PermissionIds.TasksUpdate),
            TeamLead(PermissionIds.TasksAssign),

            Member(PermissionIds.OrganizationsRead),
            Member(PermissionIds.ProjectsRead),
            Member(PermissionIds.TasksRead),
            Member(PermissionIds.TasksCreate),
            Member(PermissionIds.TasksUpdate),

            Viewer(PermissionIds.OrganizationsRead),
            Viewer(PermissionIds.MembersRead),
            Viewer(PermissionIds.ProjectsRead),
            Viewer(PermissionIds.TasksRead),
            Viewer(PermissionIds.ReportsRead));
    }

    private static object Owner(Guid permissionId) =>
        new
        {
            RoleId = SystemRoles.OwnerId,
            PermissionId = permissionId
        };

    private static object Admin(Guid permissionId) =>
        new
        {
            RoleId = SystemRoles.AdminId,
            PermissionId = permissionId
        };

    private static object ProjectManager(Guid permissionId) =>
        new
        {
            RoleId = SystemRoles.ProjectManagerId,
            PermissionId = permissionId
        };

    private static object TeamLead(Guid permissionId) =>
        new
        {
            RoleId = SystemRoles.TeamLeadId,
            PermissionId = permissionId
        };

    private static object Member(Guid permissionId) =>
        new
        {
            RoleId = SystemRoles.MemberId,
            PermissionId = permissionId
        };

    private static object Viewer(Guid permissionId) =>
        new
        {
            RoleId = SystemRoles.ViewerId,
            PermissionId = permissionId
        };
}
