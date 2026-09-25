namespace OpsFlow.Application.Features.Files.ListFiles;

public sealed record ListFilesQuery(
    int Page = 1,
    int PageSize = 20);
