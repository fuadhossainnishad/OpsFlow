using OpsFlow.Domain.Files;

namespace OpsFlow.Application.Abstractions.Files;

public interface IFileRepository
{
    Task AddAsync(
        FileRecord file,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FileRecord>> ListAsync(
        Guid organizationId,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<FileRecord?> GetAsync(
        Guid organizationId,
        Guid fileId,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        FileRecord file,
        CancellationToken cancellationToken);
}
