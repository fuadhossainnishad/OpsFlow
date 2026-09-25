using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.Common;

namespace OpsFlow.Application.Features.Tasks.ChangeTaskStatus;

public sealed class ChangeTaskStatusHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITaskRepository taskRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<ChangeTaskStatusResult> HandleAsync(
        ChangeTaskStatusCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId = await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var task = await taskRepository.GetByIdAsync(
            organizationId, command.TaskId, cancellationToken);

        if (task is null)
            throw new NotFoundException("Task was not found.");

        TaskConcurrency.EnsureCurrent(task, command.RowVersion);

        var beforeJson = JsonSerializer.Serialize(new
        {
            task.Id,
            Status = task.Status.ToString()
        });

        task.ChangeStatus(command.Status);

        var afterJson = JsonSerializer.Serialize(new
        {
            task.Id,
            Status = task.Status.ToString()
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "task.status_changed",
            "task",
            task.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeTaskStatusResult(
            task.Id,
            task.OrganizationId,
            task.Status,
            Convert.ToBase64String(task.RowVersion));
    }
}
