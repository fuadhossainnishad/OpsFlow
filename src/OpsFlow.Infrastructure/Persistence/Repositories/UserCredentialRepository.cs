using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class UserCredentialRepository(
    OpsFlowDbContext dbContext) : IUserCredentialRepository
{
    public async Task AddAsync(
        UserCredential credential,
        CancellationToken cancellationToken)
    {
        await dbContext.UserCredentials.AddAsync(
            credential,
            cancellationToken);
    }

    public Task<UserCredential?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.UserCredentials
            .SingleOrDefaultAsync(
                credential => credential.UserId == userId,
                cancellationToken);
    }
}
