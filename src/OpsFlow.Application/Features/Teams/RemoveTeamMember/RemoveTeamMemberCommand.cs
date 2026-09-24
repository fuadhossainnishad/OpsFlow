namespace OpsFlow.Application.Features.Teams.RemoveTeamMember;

public sealed record RemoveTeamMemberCommand(
    Guid TeamId,
    Guid MembershipId);
