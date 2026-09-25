namespace OpsFlow.Contracts.Projects;

public sealed record ListProjectResponse(
    Guid Id,
    string Name,
    string Key,
    string? Description,
    string Status);
