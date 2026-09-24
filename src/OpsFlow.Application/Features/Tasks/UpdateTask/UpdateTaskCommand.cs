namespace OpsFlow.Application.Features.Tasks.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid TaskId,
    string Title,
    string? Description,
    string RowVersion);
