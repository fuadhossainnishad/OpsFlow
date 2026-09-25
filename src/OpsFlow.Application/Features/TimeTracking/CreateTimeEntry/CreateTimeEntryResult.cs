namespace OpsFlow.Application.Features.TimeTracking.CreateTimeEntry;

public sealed record CreateTimeEntryResult(
    Guid TimeEntryId,
    Guid OrganizationId,
    Guid UserId,
    Guid ProjectId,
    Guid? TaskId,
    string? Description,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? EndedAtUtc,
    long? DurationSeconds,
    bool IsManual);
