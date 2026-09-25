namespace OpsFlow.Application.Features.TimeTracking.GetTimeEntry;

public sealed record GetTimeEntryResult(
    Guid TimeEntryId,
    Guid OrganizationId,
    Guid UserId,
    Guid ProjectId,
    Guid? TaskId,
    string? Description,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? EndedAtUtc,
    long? DurationSeconds,
    bool IsManual,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
