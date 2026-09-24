using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Tasks.GetTask;

public sealed class GetTaskHandler(
    ITenantContext tenantContext,
    ITaskRepository taskRepository)
{
    public async Task<GetTaskResult> HandleAsync(
        GetTaskQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var task = await taskRepository.GetByIdAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            query.TaskId,
            cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task was not found.");
        }

        return new GetTaskResult(
            task.Id,
            task.OrganizationId,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssigneeUserId,
            task.Status,
            task.CreatedAtUtc,
            task.UpdatedAtUtc);
    }
}
