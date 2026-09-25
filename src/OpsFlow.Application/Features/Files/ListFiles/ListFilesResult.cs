namespace OpsFlow.Application.Features.Files.ListFiles;

public sealed record FileItem(
    Guid Id,
    Guid OrganizationId,
    Guid UploadedByUserId,
    string OriginalFileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAtUtc);

public sealed record ListFilesResult(
    IReadOnlyList<FileItem> Items,
    int Page,
    int PageSize);
