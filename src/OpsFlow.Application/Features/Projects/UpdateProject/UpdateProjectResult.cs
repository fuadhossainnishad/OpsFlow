namespace OpsFlow.Application.Features.Projects.UpdateProject;

public sealed record UpdateProjectResult(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Key,
    string? Description,
    string Status);
