namespace OpsFlow.Application.Features.Teams.AddTeamMember;

public sealed record AddTeamMemberResult(
    Guid TeamId,
    Guid MembershipId,
    Guid TeamMemberId);
