using OpsFlow.Domain.Tasks;

namespace OpsFlow.Api.Contracts.Tasks;

public sealed record AssignTaskResponse(
    Guid TaskId,
    Guid OrganizationId,
    Guid? AssigneeUserId,
    OpsFlow.Domain.Tasks.TaskStatus Status,
    string RowVersion);
