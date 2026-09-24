using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository(
    OpsFlowDbContext dbContext) : IOrganizationRepository
{
    public Task<bool> ExistsBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        return dbContext.Organizations
            .AnyAsync(
                organization => organization.Slug == slug,
                cancellationToken);
    }

    public async Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken)
    {
        await dbContext.Organizations.AddAsync(
            organization,
            cancellationToken);
    }
}
