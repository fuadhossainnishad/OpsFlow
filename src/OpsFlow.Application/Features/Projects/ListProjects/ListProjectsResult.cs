namespace OpsFlow.Application.Features.Projects.ListProjects;

public sealed record ListProjectsResult(
    IReadOnlyList<ProjectListItem> Projects);

public sealed record ProjectListItem(
    Guid Id,
    string Name,
    string Key,
    string? Description,
    string Status);
