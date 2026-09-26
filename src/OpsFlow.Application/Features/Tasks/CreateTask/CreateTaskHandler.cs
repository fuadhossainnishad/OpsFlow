using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Contracts.Events;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IProjectRepository projectRepository,
    IMembershipRepository membershipRepository,
    ITaskRepository taskRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork,
    IOutboxWriter outboxWriter)
{
    public async Task<CreateTaskResult> HandleAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId = await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var project = await projectRepository.GetByIdAsync(
            organizationId, command.ProjectId, cancellationToken);

        if (project is null)
            throw new NotFoundException("Project was not found.");

        if (command.AssigneeUserId.HasValue)
        {
            var isActiveMember = await membershipRepository.IsActiveMemberAsync(
                organizationId, command.AssigneeUserId.Value, cancellationToken);

            if (!isActiveMember)
                throw new ConflictException(
                    "The assignee is not an active member of this organization.");
        }

        var task = TaskItem.Create(
            organizationId,
            project.Id,
            command.Title,
            command.Description,
            command.AssigneeUserId);

        await taskRepository.AddAsync(task, cancellationToken);

        var afterJson = JsonSerializer.Serialize(new
        {
            task.Id,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssigneeUserId,
            Status = task.Status.ToString()
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "task.created",
            "task",
            task.Id,
            null,
            afterJson,
            cancellationToken);

        await outboxWriter.AddAsync("task.created", new TaskCreatedEvent(
                task.Id,
                task.OrganizationId,
                task.ProjectId,
                task.Title,
                task.AssigneeUserId,
                DateTimeOffset.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTaskResult(
            task.Id,
            task.OrganizationId,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssigneeUserId,
            task.Status,
            Convert.ToBase64String(task.RowVersion));
    }
}
