namespace OpsFlow.Application.Features.Reports.GetProjectReport;

public sealed record ProjectReportRow(
    Guid ProjectId,
    string ProjectKey,
    string ProjectName,
    string Status,
    int TotalTasks,
    int CompletedTasks,
    int OpenTasks,
    long TrackedSeconds,
    int TimeEntryCount);
