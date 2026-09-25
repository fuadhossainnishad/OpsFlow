namespace OpsFlow.Application.Features.TimeTracking.UpdateTimeEntry;

public sealed record UpdateTimeEntryResult(
    Guid TimeEntryId,
    Guid ProjectId,
    Guid? TaskId,
    string? Description,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? EndedAtUtc,
    long? DurationSeconds,
    bool IsManual);
