namespace OpsFlow.Api.Contracts.Teams;

public sealed record UpdateTeamRequest(
    string Name,
    string? Description);
