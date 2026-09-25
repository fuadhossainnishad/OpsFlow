namespace OpsFlow.Application.Features.TimeTracking.CreateTimeEntry;

public sealed record CreateTimeEntryCommand(
    Guid ProjectId,
    Guid? TaskId,
    string? Description,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? EndedAtUtc);
