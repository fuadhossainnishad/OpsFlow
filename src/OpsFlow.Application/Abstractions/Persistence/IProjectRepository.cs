using OpsFlow.Domain.Projects;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<bool> ExistsByKeyAsync(
        Guid organizationId,
        string key,
        CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(
           Guid organizationId,
           Guid projectId,
           CancellationToken cancellationToken);

    Task AddAsync(
        Project project,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> GetAllAsync(
        Guid organizationId,
        CancellationToken cancellationToken);
}
