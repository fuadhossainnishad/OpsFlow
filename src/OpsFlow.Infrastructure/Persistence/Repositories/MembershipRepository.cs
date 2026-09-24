using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class MembershipRepository(
    OpsFlowDbContext dbContext) : IMembershipRepository
{
    public async Task AddAsync(
        Membership membership,
        CancellationToken cancellationToken)
    {
        await dbContext.Memberships.AddAsync(
            membership,
            cancellationToken);
    }
}
