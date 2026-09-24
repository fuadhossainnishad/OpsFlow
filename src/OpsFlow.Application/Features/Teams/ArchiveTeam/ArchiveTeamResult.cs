namespace OpsFlow.Application.Features.Teams.ArchiveTeam;

public sealed record ArchiveTeamResult(
    Guid TeamId,
    bool IsArchived);
