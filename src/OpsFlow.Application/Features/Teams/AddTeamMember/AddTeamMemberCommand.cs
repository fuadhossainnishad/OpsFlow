namespace OpsFlow.Application.Features.Teams.AddTeamMember;

public sealed record AddTeamMemberCommand(
    Guid TeamId,
    Guid MembershipId);
