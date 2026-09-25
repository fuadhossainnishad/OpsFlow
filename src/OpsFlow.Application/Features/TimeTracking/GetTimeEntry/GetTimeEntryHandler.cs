using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.TimeTracking.GetTimeEntry;

public sealed class GetTimeEntryHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITimeEntryRepository timeEntryRepository)
{
    public async Task<GetTimeEntryResult> HandleAsync(
        GetTimeEntryQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var entry = await timeEntryRepository.GetByIdAsync(
            organizationId,
            currentUser.UserId,
            query.TimeEntryId,
            cancellationToken);

        if (entry is null)
            throw new NotFoundException("Time entry was not found.");

        return new GetTimeEntryResult(
            entry.Id,
            entry.OrganizationId,
            entry.UserId,
            entry.ProjectId,
            entry.TaskId,
            entry.Description,
            entry.StartedAtUtc,
            entry.EndedAtUtc,
            entry.DurationSeconds,
            entry.IsManual,
            entry.CreatedAtUtc,
            entry.UpdatedAtUtc);
    }
}
