using OpsFlow.Application.Abstractions.Reports;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Reports.GetDashboardSummary;

public sealed class GetDashboardSummaryHandler(
    IReportRepository repository,
    ITenantContext tenantContext)
{
    public async Task<DashboardSummaryResult> HandleAsync(
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        return await repository.GetDashboardAsync(
            organizationId,
            cancellationToken);
    }
}
