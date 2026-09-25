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

    public const string TimeEntriesRead = "time_entries.read";
    public const string TimeEntriesCreate = "time_entries.create";
    public const string TimeEntriesUpdate = "time_entries.update";
    public const string TimeEntriesDelete = "time_entries.delete";
    public const string NotificationsRead = "notifications.read";
    public const string ApprovalsRead = "approvals.read";
    public const string ApprovalsCreate = "approvals.create";
    public const string ApprovalsApprove = "approvals.approve";
    public const string ApprovalsReject = "approvals.reject";
    public const string ApprovalsCancel = "approvals.cancel";

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
        BillingManage,
        TeamsRead,
        TeamsCreate,
        TeamsUpdate,
        TeamsManageMembers,
        TimeEntriesRead,
        TimeEntriesCreate,
        TimeEntriesUpdate,
        TimeEntriesDelete,
        NotificationsRead,
        ApprovalsRead,
        ApprovalsCreate,
        ApprovalsApprove,
        ApprovalsReject,
        ApprovalsCancel
    ];
}
