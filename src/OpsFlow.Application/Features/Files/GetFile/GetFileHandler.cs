using OpsFlow.Application.Abstractions.Files;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Files.GetFile;

public sealed class GetFileHandler(
    IFileRepository repository,
    IFileStorage storage,
    ITenantContext tenantContext)
{
    public async Task<(Stream Content, string ContentType, string FileName, long SizeBytes)> HandleAsync(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var file = await repository.GetAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            fileId,
            cancellationToken);

        if (file is null)
            throw new KeyNotFoundException("File was not found.");

        var stream = await storage.OpenReadAsync(
            file.StorageKey,
            cancellationToken);

        if (stream is null)
            throw new KeyNotFoundException("Stored file was not found.");

        return (
            stream,
            file.ContentType,
            file.OriginalFileName,
            file.SizeBytes);
    }
}
