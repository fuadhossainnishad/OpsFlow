using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Domain.TimeTracking;

namespace OpsFlow.Application.Features.TimeTracking.CreateTimeEntry;

public sealed class CreateTimeEntryHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IProjectRepository projectRepository,
    ITaskRepository taskRepository,
    ITimeEntryRepository timeEntryRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateTimeEntryResult> HandleAsync(
        CreateTimeEntryCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        if (currentUser.UserId == Guid.Empty)
            throw new UnauthorizedException("Authenticated user is required.");

        var project = await projectRepository.GetByIdAsync(
            organizationId,
            command.ProjectId,
            cancellationToken);

        if (project is null)
            throw new NotFoundException("Project was not found.");

        if (command.TaskId.HasValue)
        {
            var task = await taskRepository.GetByIdAsync(
                organizationId,
                command.TaskId.Value,
                cancellationToken);

            if (task is null)
                throw new NotFoundException("Task was not found.");

            if (task.ProjectId != command.ProjectId)
                throw new ConflictException(
                    "The task does not belong to the selected project.");
        }

        TimeEntry entry;

        if (command.EndedAtUtc.HasValue)
        {
            var startedAt = command.StartedAtUtc
                ?? throw new ArgumentException(
                    "StartedAtUtc is required for a completed time entry.",
                    nameof(command));

            entry = TimeEntry.CreateManual(
                organizationId,
                currentUser.UserId,
                command.ProjectId,
                command.TaskId,
                command.Description,
                startedAt,
                command.EndedAtUtc.Value);
        }
        else
        {
            if (command.StartedAtUtc.HasValue)
                throw new ArgumentException(
                    "A running time entry cannot specify an explicit start time.",
                    nameof(command));

            if (await timeEntryRepository.HasRunningEntryAsync(
                    organizationId,
                    currentUser.UserId,
                    cancellationToken))
            {
                throw new ConflictException(
                    "The user already has a running time entry.");
            }

            entry = TimeEntry.Start(
                organizationId,
                currentUser.UserId,
                command.ProjectId,
                command.TaskId,
                command.Description);
        }

        await timeEntryRepository.AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTimeEntryResult(
            entry.Id,
            entry.OrganizationId,
            entry.UserId,
            entry.ProjectId,
            entry.TaskId,
            entry.Description,
            entry.StartedAtUtc,
            entry.EndedAtUtc,
            entry.DurationSeconds,
            entry.IsManual);
    }
}
