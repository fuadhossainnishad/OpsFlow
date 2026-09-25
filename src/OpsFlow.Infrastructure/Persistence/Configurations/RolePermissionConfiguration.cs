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
            Owner(PermissionIds.OrganizationsCreate),
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
            Owner(PermissionIds.TeamsRead),
Owner(PermissionIds.TeamsCreate),
Owner(PermissionIds.TeamsUpdate),
Owner(PermissionIds.TeamsManageMembers),
            Owner(PermissionIds.TimeEntriesRead),
            Owner(PermissionIds.TimeEntriesCreate),
            Owner(PermissionIds.TimeEntriesUpdate),
            Owner(PermissionIds.TimeEntriesDelete),
            Owner(PermissionIds.NotificationsRead),
            Owner(PermissionIds.FilesRead),
            Owner(PermissionIds.FilesCreate),
            Owner(PermissionIds.FilesDelete),

            Admin(PermissionIds.OrganizationsRead),
            Admin(PermissionIds.OrganizationsCreate),
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
            Admin(PermissionIds.TeamsRead),
Admin(PermissionIds.TeamsCreate),
Admin(PermissionIds.TeamsUpdate),
Admin(PermissionIds.TeamsManageMembers),
            Admin(PermissionIds.TimeEntriesRead),
            Admin(PermissionIds.TimeEntriesCreate),
            Admin(PermissionIds.TimeEntriesUpdate),
            Admin(PermissionIds.TimeEntriesDelete),
            Admin(PermissionIds.NotificationsRead),
            Admin(PermissionIds.FilesRead),
            Admin(PermissionIds.FilesCreate),
            Admin(PermissionIds.FilesDelete),

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
            ProjectManager(PermissionIds.TeamsRead),
ProjectManager(PermissionIds.TeamsCreate),
ProjectManager(PermissionIds.TeamsUpdate),
ProjectManager(PermissionIds.TeamsManageMembers),
            ProjectManager(PermissionIds.TimeEntriesRead),
            ProjectManager(PermissionIds.TimeEntriesCreate),
            ProjectManager(PermissionIds.TimeEntriesUpdate),
            ProjectManager(PermissionIds.TimeEntriesDelete),
            ProjectManager(PermissionIds.NotificationsRead),
            ProjectManager(PermissionIds.FilesRead),
            ProjectManager(PermissionIds.FilesCreate),
            ProjectManager(PermissionIds.FilesDelete),

            TeamLead(PermissionIds.OrganizationsRead),
            TeamLead(PermissionIds.MembersRead),
            TeamLead(PermissionIds.ProjectsRead),
            TeamLead(PermissionIds.TasksRead),
            TeamLead(PermissionIds.TasksCreate),
            TeamLead(PermissionIds.TasksUpdate),
            TeamLead(PermissionIds.TasksAssign),
            TeamLead(PermissionIds.TeamsRead),
TeamLead(PermissionIds.TeamsUpdate),
TeamLead(PermissionIds.TeamsManageMembers),
            TeamLead(PermissionIds.TimeEntriesRead),
            TeamLead(PermissionIds.TimeEntriesCreate),
            TeamLead(PermissionIds.TimeEntriesUpdate),
            TeamLead(PermissionIds.TimeEntriesDelete),
            TeamLead(PermissionIds.NotificationsRead),
            TeamLead(PermissionIds.FilesRead),
            TeamLead(PermissionIds.FilesCreate),
            TeamLead(PermissionIds.FilesDelete),

            Member(PermissionIds.OrganizationsRead),
            Member(PermissionIds.ProjectsRead),
            Member(PermissionIds.TasksRead),
            Member(PermissionIds.TasksCreate),
            Member(PermissionIds.TasksUpdate),
            Member(PermissionIds.TeamsRead),
            Member(PermissionIds.TimeEntriesRead),
            Member(PermissionIds.TimeEntriesCreate),
            Member(PermissionIds.TimeEntriesUpdate),
            Member(PermissionIds.TimeEntriesDelete),
            Member(PermissionIds.NotificationsRead),
            Member(PermissionIds.FilesRead),
            Member(PermissionIds.FilesCreate),

            Viewer(PermissionIds.TeamsRead),
            Viewer(PermissionIds.OrganizationsRead),
            Viewer(PermissionIds.MembersRead),
            Viewer(PermissionIds.ProjectsRead),
            Viewer(PermissionIds.TasksRead),
            Viewer(PermissionIds.ReportsRead),
            Viewer(PermissionIds.TimeEntriesRead),
            Viewer(PermissionIds.FilesRead));
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
