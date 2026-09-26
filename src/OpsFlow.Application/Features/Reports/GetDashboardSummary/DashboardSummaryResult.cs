namespace OpsFlow.Application.Features.Reports.GetDashboardSummary;

public sealed record DashboardSummaryResult(
    int TotalProjects,
    int ActiveProjects,
    int TotalTasks,
    int TodoTasks,
    int InProgressTasks,
    int DoneTasks,
    int CancelledTasks,
    int PendingApprovals,
    int RunningTimers,
    long TrackedSeconds);
