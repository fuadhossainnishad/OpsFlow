using OpsFlow.Application.Abstractions.Reports;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Reports.GetProjectReport;

public sealed class GetProjectReportHandler(
    IReportRepository repository,
    ITenantContext tenantContext)
{
    public async Task<IReadOnlyList<ProjectReportRow>> HandleAsync(
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        return await repository.GetProjectReportAsync(
            organizationId,
            cancellationToken);
    }
}
