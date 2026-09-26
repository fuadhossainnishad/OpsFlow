namespace OpsFlow.Application.Features.Reports.GetTimeReport;

public sealed record TimeReportRow(
    Guid ProjectId,
    string ProjectName,
    long TrackedSeconds,
    int EntryCount);
