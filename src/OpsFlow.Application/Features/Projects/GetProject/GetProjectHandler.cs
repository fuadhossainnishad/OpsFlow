using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Projects.GetProject;

public sealed class GetProjectHandler(
    ITenantContext tenantContext,
    IProjectRepository projectRepository)
{
    public async Task<GetProjectResult> HandleAsync(
        GetProjectQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId = await tenantContext.GetOrganizationIdAsync(
            cancellationToken);

        var project = await projectRepository.GetByIdAsync(
            organizationId,
            query.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project was not found.");
        }

        return new GetProjectResult(
            project.Id,
            project.OrganizationId,
            project.Name,
            project.Key,
            project.Description,
            project.Status.ToString());
    }
}
