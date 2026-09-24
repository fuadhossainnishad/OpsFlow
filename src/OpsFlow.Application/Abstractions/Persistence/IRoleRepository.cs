using OpsFlow.Domain.Authorization;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<Role?> GetByNormalizedNameAsync(
        string normalizedName,
        CancellationToken cancellationToken);
}
