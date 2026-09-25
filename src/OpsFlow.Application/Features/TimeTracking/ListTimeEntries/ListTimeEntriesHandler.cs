using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.TimeTracking.ListTimeEntries;

public sealed class ListTimeEntriesHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITimeEntryRepository timeEntryRepository)
{
    public async Task<ListTimeEntriesResult> HandleAsync(
        ListTimeEntriesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var items = await timeEntryRepository.GetAsync(
            organizationId,
            currentUser.UserId,
            query.ProjectId,
            query.TaskId,
            query.FromUtc,
            query.ToUtc,
            cancellationToken);

        return new ListTimeEntriesResult(items);
    }
}
