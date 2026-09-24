namespace OpsFlow.Application.Features.Members.DeactivateMember;

public sealed record DeactivateMemberCommand(
    Guid MembershipId);
