namespace OpsFlow.Application.Features.Teams.ChangeTeamLead;

public sealed record ChangeTeamLeadCommand(
    Guid TeamId,
    Guid MembershipId);
