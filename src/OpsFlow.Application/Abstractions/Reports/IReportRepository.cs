using OpsFlow.Application.Features.Reports.GetDashboardSummary;
using OpsFlow.Application.Features.Reports.GetProjectReport;
using OpsFlow.Application.Features.Reports.GetTimeReport;

namespace OpsFlow.Application.Abstractions.Reports;

public interface IReportRepository
{
    Task<DashboardSummaryResult> GetDashboardAsync(
        Guid organizationId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TimeReportRow>> GetTimeReportAsync(
        Guid organizationId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectReportRow>> GetProjectReportAsync(
        Guid organizationId,
        CancellationToken cancellationToken);
}
