using OpsFlow.Domain.Tasks;

namespace OpsFlow.Api.Contracts.Tasks;

public sealed record ChangeTaskStatusResponse(
    Guid TaskId,
    Guid OrganizationId,
    OpsFlow.Domain.Tasks.TaskStatus Status,
    string RowVersion);
