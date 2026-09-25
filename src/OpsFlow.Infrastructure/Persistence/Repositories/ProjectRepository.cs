using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Projects;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(OpsFlowDbContext dbContext) : IProjectRepository
{
    public Task<bool> ExistsByKeyAsync(
        Guid organizationId,
        string key,
        CancellationToken cancellationToken)
        => dbContext.Projects.AnyAsync(
            project => project.OrganizationId == organizationId &&
                       project.Key == key,
            cancellationToken);

    public Task<Project?> GetByIdAsync(
        Guid organizationId,
        Guid projectId,
        CancellationToken cancellationToken)
        => dbContext.Projects.SingleOrDefaultAsync(
            project => project.OrganizationId == organizationId &&
                       project.Id == projectId,
            cancellationToken);

    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken)
    {
        await dbContext.Projects.AddAsync(project, cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
        => await dbContext.Projects
            .Where(project => project.OrganizationId == organizationId)
            .OrderBy(project => project.Name)
            .ToListAsync(cancellationToken);
}
