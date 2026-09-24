using OpsFlow.Domain.Tasks;

namespace OpsFlow.Api.Contracts.Tasks;

public sealed record CreateTaskResponse(
    Guid TaskId,
    Guid OrganizationId,
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssigneeUserId,
    OpsFlow.Domain.Tasks.TaskStatus Status);
