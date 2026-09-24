namespace OpsFlow.Application.Features.Members.ChangeMemberRole;

public sealed record ChangeMemberRoleResult(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    Guid RoleId,
    string RoleName,
    bool IsActive);
