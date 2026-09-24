using OpsFlow.Domain.Identity;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IUserCredentialRepository
{
    Task AddAsync(
        UserCredential credential,
        CancellationToken cancellationToken);

    Task<UserCredential?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);
}
