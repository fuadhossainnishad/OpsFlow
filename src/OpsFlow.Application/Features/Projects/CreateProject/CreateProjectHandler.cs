using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Domain.Projects;

namespace OpsFlow.Application.Features.Projects.CreateProject;

public sealed class CreateProjectHandler(
    ITenantContext tenantContext,
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateProjectResult> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var name = command.Name.Trim();
        var key = command.Key.Trim().ToUpperInvariant();

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var organizationId = await tenantContext.GetOrganizationIdAsync(
            cancellationToken);

        var keyExists = await projectRepository.ExistsByKeyAsync(
            organizationId,
            key,
            cancellationToken);

        if (keyExists)
        {
            throw new ConflictException(
                "A project with this key already exists in the organization.");
        }

        var project = Project.Create(
            organizationId,
            name,
            key,
            command.Description);

        await projectRepository.AddAsync(
            project,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateProjectResult(
            project.Id,
            project.OrganizationId,
            project.Name,
            project.Key,
            project.Description,
            project.Status.ToString());
    }


}
