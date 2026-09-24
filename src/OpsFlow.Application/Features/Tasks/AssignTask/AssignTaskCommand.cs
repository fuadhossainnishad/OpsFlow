namespace OpsFlow.Application.Features.Tasks.AssignTask;

public sealed record AssignTaskCommand(
    Guid TaskId,
    Guid AssigneeUserId,
    string RowVersion);
