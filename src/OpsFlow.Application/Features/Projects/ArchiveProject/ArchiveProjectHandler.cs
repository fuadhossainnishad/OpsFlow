using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Projects.ArchiveProject;

public sealed class ArchiveProjectHandler(
    ITenantContext tenantContext,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        ArchiveProjectCommand command,
        CancellationToken cancellationToken)
    {
        var organizationId = await tenantContext.GetOrganizationIdAsync(
            cancellationToken);

        var project = await projectRepository.GetByIdAsync(
            organizationId,
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project was not found.");
        }

        project.Archive();

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
