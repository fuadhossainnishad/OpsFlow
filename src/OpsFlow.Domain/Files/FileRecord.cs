namespace OpsFlow.Domain.Files;

public sealed class FileRecord
{
    private FileRecord()
    {
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid UploadedByUserId { get; private set; }
    public string OriginalFileName { get; private set; } = null!;
    public string StorageKey { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }

    public static FileRecord Create(
        Guid organizationId,
        Guid uploadedByUserId,
        string originalFileName,
        string storageKey,
        string contentType,
        long sizeBytes,
        DateTimeOffset createdAtUtc)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required.", nameof(organizationId));

        if (uploadedByUserId == Guid.Empty)
            throw new ArgumentException("Uploader user ID is required.", nameof(uploadedByUserId));

        ArgumentException.ThrowIfNullOrWhiteSpace(originalFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(storageKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeBytes);

        return new FileRecord
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UploadedByUserId = uploadedByUserId,
            OriginalFileName = Path.GetFileName(originalFileName.Trim()),
            StorageKey = storageKey.Trim(),
            ContentType = contentType.Trim(),
            SizeBytes = sizeBytes,
            CreatedAtUtc = createdAtUtc
        };
    }
}
