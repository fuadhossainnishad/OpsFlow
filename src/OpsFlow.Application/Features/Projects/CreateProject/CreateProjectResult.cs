namespace OpsFlow.Application.Features.Projects.CreateProject;

public sealed record CreateProjectResult(
    Guid ProjectId,
    Guid OrganizationId,
    string Name,
    string Key,
    string? Description,
    string Status);
