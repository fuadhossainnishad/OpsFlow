using OpsFlow.Application.Abstractions.Files;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Files.ListFiles;

public sealed class ListFilesHandler(
    IFileRepository repository,
    ITenantContext tenantContext)
{
    public async Task<ListFilesResult> HandleAsync(
        ListFilesQuery query,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var files = await repository.ListAsync(
            await tenantContext.GetOrganizationIdAsync(cancellationToken),
            (page - 1) * pageSize,
            pageSize,
            cancellationToken);

        return new ListFilesResult(
            files.Select(x => new FileItem(
                x.Id,
                x.OrganizationId,
                x.UploadedByUserId,
                x.OriginalFileName,
                x.ContentType,
                x.SizeBytes,
                x.CreatedAtUtc)).ToList(),
            page,
            pageSize);
    }
}
