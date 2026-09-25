using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.Common;

namespace OpsFlow.Application.Features.Tasks.AssignTask;

public sealed class AssignTaskHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITaskRepository taskRepository,
    IMembershipRepository membershipRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<AssignTaskResult> HandleAsync(
        AssignTaskCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId = await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var task = await taskRepository.GetByIdAsync(
            organizationId, command.TaskId, cancellationToken);

        if (task is null)
            throw new NotFoundException("Task was not found.");

        TaskConcurrency.EnsureCurrent(task, command.RowVersion);

        var isActiveMember = await membershipRepository.IsActiveMemberAsync(
            organizationId, command.AssigneeUserId, cancellationToken);

        if (!isActiveMember)
            throw new NotFoundException(
                "Assignee was not found in the selected organization.");

        var beforeJson = JsonSerializer.Serialize(new
        {
            task.Id,
            task.AssigneeUserId
        });

        task.AssignTo(command.AssigneeUserId);

        var afterJson = JsonSerializer.Serialize(new
        {
            task.Id,
            task.AssigneeUserId
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "task.assigned",
            "task",
            task.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AssignTaskResult(
            task.Id,
            task.OrganizationId,
            task.AssigneeUserId,
            task.Status,
            Convert.ToBase64String(task.RowVersion));
    }
}
