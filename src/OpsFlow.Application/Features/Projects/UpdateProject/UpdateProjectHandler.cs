using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Projects.UpdateProject;

public sealed class UpdateProjectHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IProjectRepository projectRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdateProjectResult> HandleAsync(
        UpdateProjectCommand command,
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
            throw new NotFoundException("Project was not found.");
        }

        var beforeJson = JsonSerializer.Serialize(new
        {
            project.Id,
            project.Name,
            project.Key,
            project.Description,
            Status = project.Status.ToString()
        });

        project.Update(command.Name, command.Description);

        var afterJson = JsonSerializer.Serialize(new
        {
            project.Id,
            project.Name,
            project.Key,
            project.Description,
            Status = project.Status.ToString()
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "project.updated",
            "project",
            project.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateProjectResult(
            project.Id,
            project.OrganizationId,
            project.Name,
            project.Key,
            project.Description,
            project.Status.ToString());
    }
}
