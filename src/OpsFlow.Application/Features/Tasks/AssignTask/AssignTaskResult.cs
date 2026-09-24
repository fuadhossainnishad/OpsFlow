namespace OpsFlow.Application.Features.Tasks.AssignTask;

public sealed record AssignTaskResult(
    Guid TaskId,
    Guid OrganizationId,
    Guid? AssigneeUserId,
    OpsFlow.Domain.Tasks.TaskStatus Status,
    string RowVersion);
