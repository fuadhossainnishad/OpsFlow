using OpsFlow.Application.Abstractions.Reports;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Reports.GetTimeReport;

public sealed class GetTimeReportHandler(
    IReportRepository repository,
    ITenantContext tenantContext)
{
    public async Task<IReadOnlyList<TimeReportRow>> HandleAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        if (to < from)
        {
            throw new ArgumentException(
                "The report end date must be on or after the start date.",
                nameof(to));
        }

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var fromUtc = new DateTimeOffset(
            from.ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero);

        var toExclusiveUtc = new DateTimeOffset(
            to.AddDays(1).ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero);

        return await repository.GetTimeReportAsync(
            organizationId,
            fromUtc,
            toExclusiveUtc,
            cancellationToken);
    }
}
