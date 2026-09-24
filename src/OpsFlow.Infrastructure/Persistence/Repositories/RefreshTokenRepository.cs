using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(
    OpsFlowDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        await dbContext.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }
}
