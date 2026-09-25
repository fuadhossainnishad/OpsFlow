namespace OpsFlow.Application.Features.TimeTracking.ListTimeEntries;

public sealed record ListTimeEntriesResult(
    IReadOnlyList<TimeEntryRecord> Items);
