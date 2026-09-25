namespace OpsFlow.Api.Contracts.TimeTracking;

public sealed record TimeEntryResponse(
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
