namespace OpsFlow.Application.Features.Tasks.GetTask;

public sealed record GetTaskResult(
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
