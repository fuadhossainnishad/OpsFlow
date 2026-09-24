namespace OpsFlow.Application.Features.Teams.ListTeams;

public sealed record ListTeamsResult(
    IReadOnlyList<TeamItemResult> Teams);

public sealed record TeamItemResult(
    Guid TeamId,
    string Name,
    string? Description,
    Guid? TeamLeadMembershipId,
    bool IsArchived,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
