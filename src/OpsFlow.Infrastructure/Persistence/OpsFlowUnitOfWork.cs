using OpsFlow.Application.Abstractions.Persistence;

namespace OpsFlow.Infrastructure.Persistence;

public sealed class OpsFlowUnitOfWork(
    OpsFlowDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
