using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.TimeTracking.UpdateTimeEntry;

public sealed class UpdateTimeEntryHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITimeEntryRepository timeEntryRepository,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdateTimeEntryResult> HandleAsync(
        UpdateTimeEntryCommand command,
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

        if (command.TaskId.HasValue)
        {
            var task = await taskRepository.GetByIdAsync(
                organizationId,
                command.TaskId.Value,
                cancellationToken);

            if (task is null)
                throw new NotFoundException("Task was not found.");

            if (task.ProjectId != entry.ProjectId)
                throw new ConflictException(
                    "The task does not belong to the time entry project.");
        }

        entry.Update(
            command.TaskId,
            command.Description,
            command.StartedAtUtc,
            command.EndedAtUtc);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateTimeEntryResult(
            entry.Id,
            entry.ProjectId,
            entry.TaskId,
            entry.Description,
            entry.StartedAtUtc,
            entry.EndedAtUtc,
            entry.DurationSeconds,
            entry.IsManual);
    }
}
