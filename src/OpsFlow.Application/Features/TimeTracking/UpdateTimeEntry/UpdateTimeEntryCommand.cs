namespace OpsFlow.Application.Features.TimeTracking.UpdateTimeEntry;

public sealed record UpdateTimeEntryCommand(
    Guid TimeEntryId,
    Guid? TaskId,
    string? Description,
    DateTimeOffset StartedAtUtc,
    DateTimeOffset? EndedAtUtc);
