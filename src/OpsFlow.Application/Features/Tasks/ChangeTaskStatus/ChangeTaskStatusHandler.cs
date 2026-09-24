using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.Common;

namespace OpsFlow.Application.Features.Tasks.ChangeTaskStatus;

public sealed class ChangeTaskStatusHandler(
    ITenantContext tenantContext,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ChangeTaskStatusResult> HandleAsync(
        ChangeTaskStatusCommand command,
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

        task.ChangeStatus(command.Status);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeTaskStatusResult(
            task.Id,
            task.OrganizationId,
            task.Status,
            Convert.ToBase64String(task.RowVersion));
    }
}
