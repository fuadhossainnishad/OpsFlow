using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IOrganizationRepository
{
    Task<bool> ExistsBySlugAsync(
        string slug,
        CancellationToken cancellationToken);

    Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken);
}
