namespace OpsFlow.Api.Contracts.TimeTracking;

public sealed record UpdateTimeEntryRequest(
    Guid? TaskId,
    string? Description,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? EndedAtUtc);
