namespace OpsFlow.Application.Features.Teams.CreateTeam;

public sealed record CreateTeamCommand(
    string Name,
    string? Description);
