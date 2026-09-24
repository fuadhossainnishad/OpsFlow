namespace OpsFlow.Application.Features.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssigneeUserId);
