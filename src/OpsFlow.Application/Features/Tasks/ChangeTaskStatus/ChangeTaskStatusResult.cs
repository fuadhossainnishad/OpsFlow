namespace OpsFlow.Application.Features.Tasks.ChangeTaskStatus;

public sealed record ChangeTaskStatusResult(
    Guid TaskId,
    Guid OrganizationId,
    OpsFlow.Domain.Tasks.TaskStatus Status,
    string RowVersion);
