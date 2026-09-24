namespace OpsFlow.Api.Contracts.Members;

public sealed record ChangeMemberRoleResponse(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId,
    string RoleName,
    bool IsActive);
