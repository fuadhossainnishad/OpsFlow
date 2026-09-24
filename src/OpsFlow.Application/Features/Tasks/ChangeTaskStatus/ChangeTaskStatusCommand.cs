namespace OpsFlow.Application.Features.Tasks.ChangeTaskStatus;

public sealed record ChangeTaskStatusCommand(
    Guid TaskId,
    OpsFlow.Domain.Tasks.TaskStatus Status,
    string RowVersion);
