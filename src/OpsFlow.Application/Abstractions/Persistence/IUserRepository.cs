using OpsFlow.Domain.Identity;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<User?> GetByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);
}
