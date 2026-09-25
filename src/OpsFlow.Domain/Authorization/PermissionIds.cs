namespace OpsFlow.Domain.Authorization;

public static class PermissionIds
{
    public static readonly Guid OrganizationsRead =
        Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly Guid OrganizationsCreate =
        Guid.Parse("00000000-0000-0000-0000-000000000002");

    public static readonly Guid OrganizationsUpdate =
        Guid.Parse("00000000-0000-0000-0000-000000000003");

    public static readonly Guid MembersRead =
        Guid.Parse("00000000-0000-0000-0000-000000000004");

    public static readonly Guid MembersInvite =
        Guid.Parse("00000000-0000-0000-0000-000000000005");

    public static readonly Guid MembersManage =
        Guid.Parse("00000000-0000-0000-0000-000000000006");

    public static readonly Guid ProjectsRead =
        Guid.Parse("00000000-0000-0000-0000-000000000007");

    public static readonly Guid ProjectsCreate =
        Guid.Parse("00000000-0000-0000-0000-000000000008");

    public static readonly Guid ProjectsUpdate =
        Guid.Parse("00000000-0000-0000-0000-000000000009");

    public static readonly Guid TasksRead =
        Guid.Parse("00000000-0000-0000-0000-000000000010");

    public static readonly Guid TasksCreate =
        Guid.Parse("00000000-0000-0000-0000-000000000011");

    public static readonly Guid TasksUpdate =
        Guid.Parse("00000000-0000-0000-0000-000000000012");

    public static readonly Guid TasksAssign =
        Guid.Parse("00000000-0000-0000-0000-000000000013");

    public static readonly Guid ReportsRead =
        Guid.Parse("00000000-0000-0000-0000-000000000014");

    public static readonly Guid BillingManage =
        Guid.Parse("00000000-0000-0000-0000-000000000015");

    public static readonly Guid TeamsRead =
        Guid.Parse("00000000-0000-0000-0000-000000000016");

    public static readonly Guid TeamsCreate =
        Guid.Parse("00000000-0000-0000-0000-000000000017");

    public static readonly Guid TeamsUpdate =
        Guid.Parse("00000000-0000-0000-0000-000000000018");

    public static readonly Guid TeamsManageMembers =
        Guid.Parse("00000000-0000-0000-0000-000000000019");

    public static readonly Guid TimeEntriesRead =
        Guid.Parse("00000000-0000-0000-0000-000000000020");

    public static readonly Guid TimeEntriesCreate =
        Guid.Parse("00000000-0000-0000-0000-000000000021");

    public static readonly Guid TimeEntriesUpdate =
        Guid.Parse("00000000-0000-0000-0000-000000000022");

    public static readonly Guid TimeEntriesDelete =
        Guid.Parse("00000000-0000-0000-0000-000000000023");

    public static readonly Guid ApprovalsRead =
        Guid.Parse("77777777-7777-7777-7777-777777777771");

    public static readonly Guid ApprovalsCreate =
        Guid.Parse("77777777-7777-7777-7777-777777777772");

    public static readonly Guid ApprovalsApprove =
        Guid.Parse("77777777-7777-7777-7777-777777777773");

    public static readonly Guid ApprovalsReject =
        Guid.Parse("77777777-7777-7777-7777-777777777774");

    public static readonly Guid ApprovalsCancel =
        Guid.Parse("77777777-7777-7777-7777-777777777775");
    
    public static readonly Guid NotificationsRead =
        Guid.Parse("77777777-7777-7777-7777-777777777776");

    public static readonly Guid FilesRead =
        Guid.Parse("77777777-7777-7777-7777-777777777777");

    public static readonly Guid FilesCreate =
        Guid.Parse("77777777-7777-7777-7777-777777777778");

    public static readonly Guid FilesDelete =
        Guid.Parse("77777777-7777-7777-7777-777777777779");
}
