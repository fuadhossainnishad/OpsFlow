using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Projects.ArchiveProject;

public sealed class ArchiveProjectHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IProjectRepository projectRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        ArchiveProjectCommand command,
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
            Status = project.Status.ToString(),
            project.ArchivedAtUtc
        });

        project.Archive();

        var afterJson = JsonSerializer.Serialize(new
        {
            project.Id,
            project.Name,
            project.Key,
            project.Description,
            Status = project.Status.ToString(),
            project.ArchivedAtUtc
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "project.archived",
            "project",
            project.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
