namespace OpsFlow.Api.Contracts.TimeTracking;

public sealed record CreateTimeEntryRequest(
    Guid ProjectId,
    Guid? TaskId,
    string? Description,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? EndedAtUtc);
