namespace OpsFlow.Application.Features.Members.ReactivateMember;

public sealed record ReactivateMemberResult(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    bool IsActive);
