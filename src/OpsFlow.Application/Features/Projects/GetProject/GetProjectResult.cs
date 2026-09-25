namespace OpsFlow.Application.Features.Projects.GetProject;

public sealed record GetProjectResult(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Key,
    string? Description,
    string Status);
