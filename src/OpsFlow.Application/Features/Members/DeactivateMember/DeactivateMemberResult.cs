namespace OpsFlow.Application.Features.Members.DeactivateMember;

public sealed record DeactivateMemberResult(
    Guid MembershipId,
    Guid OrganizationId,
    Guid UserId,
    bool IsActive);
