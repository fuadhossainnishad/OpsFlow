using OpsFlow.Domain.Tasks;

namespace OpsFlow.Api.Contracts.Tasks;

public sealed record UpdateTaskResponse(
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
