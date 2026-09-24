namespace OpsFlow.Application.Features.Teams.ChangeTeamLead;

public sealed record ChangeTeamLeadResult(
    Guid TeamId,
    Guid TeamLeadMembershipId);
