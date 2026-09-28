namespace OpsFlow.Application.Features.TimeTracking.ListTimeEntries;

public sealed record ListTimeEntriesQuery(
    Guid? ProjectId,
    Guid? TaskId,
    DateTimeOffset? FromUtc,
    DateTimeOffset? ToUtc,
    int Page = 1,
    int PageSize = 20,
    TimeEntrySortField SortBy = TimeEntrySortField.StartedAtUtc,
    TimeEntrySortOrder SortOrder = TimeEntrySortOrder.Desc);

public enum TimeEntrySortField
{
    StartedAtUtc,
    CreatedAtUtc
}

public enum TimeEntrySortOrder
{
    Asc,
    Desc
}
