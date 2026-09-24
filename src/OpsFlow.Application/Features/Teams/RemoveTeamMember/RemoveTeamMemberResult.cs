namespace OpsFlow.Application.Features.Teams.RemoveTeamMember;

public sealed record RemoveTeamMemberResult(
    Guid TeamId,
    Guid MembershipId);
