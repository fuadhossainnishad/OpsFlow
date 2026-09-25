using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Projects.UpdateProject;

public sealed class UpdateProjectHandler(
    ITenantContext tenantContext,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<UpdateProjectResult> HandleAsync(
        UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

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

        project.Update(command.Name, command.Description);

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
