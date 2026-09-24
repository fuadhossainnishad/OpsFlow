using OpsFlow.Domain.Identity;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);
}
