namespace OpsFlow.Contracts.Projects;

public sealed record ProjectResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string Key,
    string? Description,
    string Status);
