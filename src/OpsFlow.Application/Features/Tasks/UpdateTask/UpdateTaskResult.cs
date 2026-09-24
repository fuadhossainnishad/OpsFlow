namespace OpsFlow.Application.Features.Tasks.UpdateTask;

public sealed record UpdateTaskResult(
    Guid TaskId,
    Guid OrganizationId,
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssigneeUserId,
    OpsFlow.Domain.Tasks.TaskStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc,
    string RowVersion);
