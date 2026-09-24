namespace OpsFlow.Api.Contracts.Projects;

public sealed record CreateProjectResponse(
    Guid ProjectId,
    Guid OrganizationId,
    string Name,
    string Key,
    string? Description,
    string Status);
