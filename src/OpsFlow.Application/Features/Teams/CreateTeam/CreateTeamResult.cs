namespace OpsFlow.Application.Features.Teams.CreateTeam;

public sealed record CreateTeamResult(
    Guid TeamId,
    Guid OrganizationId,
    string Name,
    string? Description,
    bool IsArchived,
    DateTimeOffset CreatedAtUtc);
