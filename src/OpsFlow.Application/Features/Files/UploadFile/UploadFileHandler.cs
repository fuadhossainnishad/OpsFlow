using OpsFlow.Application.Abstractions.Files;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Domain.Files;

namespace OpsFlow.Application.Features.Files.UploadFile;

public sealed class UploadFileHandler(
    IFileRepository repository,
    IFileStorage storage,
    ICurrentUser currentUser,
    ITenantContext tenantContext,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    private const long MaxFileSize = 25L * 1024 * 1024;

    public async Task<UploadFileResult> HandleAsync(
        Stream content,
        string fileName,
        string contentType,
        long sizeBytes,
        CancellationToken cancellationToken)
    {
        if (sizeBytes <= 0)
            throw new ArgumentException("File must not be empty.", nameof(sizeBytes));

        if (sizeBytes > MaxFileSize)
            throw new ArgumentException("File size cannot exceed 25 MB.", nameof(sizeBytes));

        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        var organizationId = await tenantContext.GetOrganizationIdAsync(cancellationToken);
        var userId = currentUser.UserId;

        var fileId = Guid.NewGuid();
        var safeName = Path.GetFileName(fileName.Trim());
        var storageKey = $"{organizationId:N}/{fileId:N}";

        await storage.SaveAsync(content, storageKey, cancellationToken);

        var file = FileRecord.Create(
            organizationId,
            userId,
            safeName,
            storageKey,
            contentType,
            sizeBytes,
            DateTimeOffset.UtcNow);

        try
        {
            await repository.AddAsync(file, cancellationToken);

            await auditLogger.LogAsync(
                organizationId,
                userId,
                "file.uploaded",
                "file",
                file.Id,
                null,
                $"{{\"fileName\":{System.Text.Json.JsonSerializer.Serialize(file.OriginalFileName)},\"sizeBytes\":{file.SizeBytes}}}",
                cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await storage.DeleteAsync(storageKey, CancellationToken.None);
            throw;
        }

        return new UploadFileResult(
            file.Id,
            file.OrganizationId,
            file.UploadedByUserId,
            file.OriginalFileName,
            file.ContentType,
            file.SizeBytes,
            file.CreatedAtUtc);
    }
}
