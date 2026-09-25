namespace OpsFlow.Application.Features.Files.UploadFile;

public sealed record UploadFileResult(
    Guid Id,
    Guid OrganizationId,
    Guid UploadedByUserId,
    string OriginalFileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAtUtc);
