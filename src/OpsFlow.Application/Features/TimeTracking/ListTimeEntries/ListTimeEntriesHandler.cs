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

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        if (!Enum.IsDefined(query.SortBy) || !Enum.IsDefined(query.SortOrder))
        {
            throw new ArgumentException("Unsupported time-entry sort option.");
        }

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var items = await timeEntryRepository.GetAsync(
            organizationId,
            currentUser.UserId,
            query.ProjectId,
            query.TaskId,
            query.FromUtc,
            query.ToUtc,
            (int)Math.Min((long)(page - 1) * pageSize, int.MaxValue),
            pageSize + 1,
            query.SortBy,
            query.SortOrder,
            cancellationToken);

        var hasNextPage = items.Count > pageSize;

        return new ListTimeEntriesResult(
            items.Take(pageSize).ToArray(),
            page,
            pageSize,
            hasNextPage);
    }
}
