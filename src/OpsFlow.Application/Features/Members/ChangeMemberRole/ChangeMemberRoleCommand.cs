namespace OpsFlow.Application.Features.Members.ChangeMemberRole;

public sealed record ChangeMemberRoleCommand(
    Guid MembershipId,
    string RoleName);
