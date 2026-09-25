using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Abstractions.Files;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;

namespace OpsFlow.Application.Features.Files.DeleteFile;

public sealed class DeleteFileHandler(
    IFileRepository repository,
    IFileStorage storage,
    ICurrentUser currentUser,
    ITenantContext tenantContext,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        Guid fileId,
        CancellationToken cancellationToken)
    {
        var file = await repository.GetAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            fileId,
            cancellationToken);

        if (file is null)
            throw new KeyNotFoundException("File was not found.");

        await storage.DeleteAsync(file.StorageKey, cancellationToken);
        await repository.DeleteAsync(file, cancellationToken);

        await auditLogger.LogAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            currentUser.UserId,
            "file.deleted",
            "file",
            file.Id,
            $"{{\"fileName\":{System.Text.Json.JsonSerializer.Serialize(file.OriginalFileName)}}}",
            null,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
