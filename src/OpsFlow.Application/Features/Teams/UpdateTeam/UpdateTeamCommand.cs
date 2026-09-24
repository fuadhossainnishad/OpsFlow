namespace OpsFlow.Application.Features.Teams.UpdateTeam;

public sealed record UpdateTeamCommand(
    Guid TeamId,
    string Name,
    string? Description);
