namespace OpsFlow.Application.Authorization;

public static class PermissionCodes
{
    public const string OrganizationsRead = "organizations.read";
    public const string OrganizationsUpdate = "organizations.update";
    public const string OrganizationsCreate = "organizations.create";

    public const string MembersRead = "members.read";
    public const string MembersInvite = "members.invite";
    public const string MembersManage = "members.manage";

    public const string ProjectsRead = "projects.read";
    public const string ProjectsCreate = "projects.create";
    public const string ProjectsUpdate = "projects.update";

    public const string TasksRead = "tasks.read";
    public const string TasksCreate = "tasks.create";
    public const string TasksUpdate = "tasks.update";
    public const string TasksAssign = "tasks.assign";

    public const string ReportsRead = "reports.read";
    public const string BillingManage = "billing.manage";

    public const string TeamsRead = "teams.read";
    public const string TeamsCreate = "teams.create";
    public const string TeamsUpdate = "teams.update";
    public const string TeamsManageMembers = "teams.manage_members";

    public static IReadOnlyCollection<string> All =>
    [
        OrganizationsRead,
        OrganizationsCreate,
        OrganizationsUpdate,
        MembersRead,
        MembersInvite,
        MembersManage,
        ProjectsRead,
        ProjectsCreate,
        ProjectsUpdate,
        TasksRead,
        TasksCreate,
        TasksUpdate,
        TasksAssign,
        ReportsRead,
        BillingManage
    ];
}
