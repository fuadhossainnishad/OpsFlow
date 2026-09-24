namespace OpsFlow.Application.Features.Projects.CreateProject;

public sealed record CreateProjectCommand(
    string Name,
    string Key,
    string? Description);
