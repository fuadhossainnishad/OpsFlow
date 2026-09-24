namespace OpsFlow.Application.Features.Tasks.CreateTask;

public sealed record CreateTaskResult(
    Guid TaskId,
    Guid OrganizationId,
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssigneeUserId,
    OpsFlow.Domain.Tasks.TaskStatus Status);
