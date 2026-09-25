using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Files;
using OpsFlow.Domain.Files;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class FileRepository(OpsFlowDbContext dbContext) : IFileRepository
{
    public async Task AddAsync(
        FileRecord file,
        CancellationToken cancellationToken)
    {
        await dbContext.Files.AddAsync(file, cancellationToken);
    }

    public async Task<IReadOnlyList<FileRecord>> ListAsync(
        Guid organizationId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        return await dbContext.Files
            .AsNoTracking()
            .Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<FileRecord?> GetAsync(
        Guid organizationId,
        Guid fileId,
        CancellationToken cancellationToken) =>
        dbContext.Files.SingleOrDefaultAsync(
            x => x.OrganizationId == organizationId && x.Id == fileId,
            cancellationToken);

    public Task DeleteAsync(
        FileRecord file,
        CancellationToken cancellationToken)
    {
        dbContext.Files.Remove(file);
        return Task.CompletedTask;
    }
}
