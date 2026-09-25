using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Projects.ListProjects;

public sealed class ListProjectsHandler(
    ITenantContext tenantContext,
    IProjectRepository projectRepository)
{
    public async Task<ListProjectsResult> HandleAsync(
        ListProjectsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var organizationId = await tenantContext.GetOrganizationIdAsync(
            cancellationToken);

        var projects = await projectRepository.GetAllAsync(
            organizationId,
            cancellationToken);

        return new ListProjectsResult(
            projects.Select(project => new ProjectListItem(
                project.Id,
                project.Name,
                project.Key,
                project.Description,
                project.Status.ToString())).ToList());
    }
}
