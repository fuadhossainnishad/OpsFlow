using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.Common;

namespace OpsFlow.Application.Features.Tasks.UpdateTask;

public sealed class UpdateTaskHandler(
    ITenantContext tenantContext,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdateTaskResult> HandleAsync(
        UpdateTaskCommand command,
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

        task.Update(command.Title, command.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateTaskResult(
            task.Id,
            task.OrganizationId,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssigneeUserId,
            task.Status,
            task.CreatedAtUtc,
            task.UpdatedAtUtc,
            Convert.ToBase64String(task.RowVersion));
    }
}
