using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.Common;

namespace OpsFlow.Application.Features.Tasks.AssignTask;

public sealed class AssignTaskHandler(
    ITenantContext tenantContext,
    ITaskRepository taskRepository,
    IMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<AssignTaskResult> HandleAsync(
        AssignTaskCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId = await tenantContext.GetOrganizationIdAsync(
            cancellationToken);

        var task = await taskRepository.GetByIdAsync(
            organizationId,
            command.TaskId,
            cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task was not found.");
        }

        TaskConcurrency.EnsureCurrent(task, command.RowVersion);

        var isActiveMember = await membershipRepository.IsActiveMemberAsync(
            organizationId,
            command.AssigneeUserId,
            cancellationToken);

        if (!isActiveMember)
        {
            throw new NotFoundException(
                "Assignee was not found in the selected organization.");
        }

        task.AssignTo(command.AssigneeUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AssignTaskResult(
            task.Id,
            task.OrganizationId,
            task.AssigneeUserId,
            task.Status,
            Convert.ToBase64String(task.RowVersion));
    }
}
