namespace OpsFlow.Application.Features.TimeTracking.ListTimeEntries;

public sealed record ListTimeEntriesQuery(
    Guid? ProjectId,
    Guid? TaskId,
    DateTimeOffset? FromUtc,
    DateTimeOffset? ToUtc);
