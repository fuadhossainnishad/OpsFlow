using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(
    OpsFlowDbContext dbContext) : IRoleRepository
{
    public Task<Role?> GetByNormalizedNameAsync(
        string normalizedName,
        CancellationToken cancellationToken)
    {
        return dbContext.Roles
            .SingleOrDefaultAsync(
                role => role.NormalizedName == normalizedName,
                cancellationToken);
    }
}
