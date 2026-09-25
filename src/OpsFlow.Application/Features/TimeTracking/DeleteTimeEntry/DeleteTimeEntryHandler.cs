using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.TimeTracking.DeleteTimeEntry;

public sealed class DeleteTimeEntryHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITimeEntryRepository timeEntryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        DeleteTimeEntryCommand command,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var entry = await timeEntryRepository.GetByIdAsync(
            organizationId,
            currentUser.UserId,
            command.TimeEntryId,
            cancellationToken);

        if (entry is null)
            throw new NotFoundException("Time entry was not found.");

        timeEntryRepository.Remove(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
