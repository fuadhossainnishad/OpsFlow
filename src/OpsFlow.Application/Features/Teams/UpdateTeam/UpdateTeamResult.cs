namespace OpsFlow.Application.Features.Teams.UpdateTeam;

public sealed record UpdateTeamResult(
    Guid TeamId,
    string Name,
    string? Description,
    DateTimeOffset? UpdatedAtUtc);
