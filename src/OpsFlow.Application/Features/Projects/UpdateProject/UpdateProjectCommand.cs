namespace OpsFlow.Application.Features.Projects.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string? Description);
