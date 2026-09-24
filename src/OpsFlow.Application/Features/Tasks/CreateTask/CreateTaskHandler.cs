using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.Application.Features.Tasks.CreateTask;

public sealed class CreateTaskHandler(
    ITenantContext tenantContext,
    IProjectRepository projectRepository,
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateTaskResult> HandleAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId = await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var project = await projectRepository.GetByIdAsync(
            organizationId,
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException(
                "Project was not found.");
        }

        var task = TaskItem.Create(
            organizationId,
            project.Id,
            command.Title,
            command.Description,
            command.AssigneeUserId);

        await taskRepository.AddAsync(
            task,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateTaskResult(
            task.Id,
            task.OrganizationId,
            task.ProjectId,
            task.Title,
            task.Description,
            task.AssigneeUserId,
            task.Status);
    }
}
