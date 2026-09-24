using OpsFlow.Domain.Tasks;

namespace OpsFlow.Api.Contracts.Tasks;

public sealed record ChangeTaskStatusRequest(
    OpsFlow.Domain.Tasks.TaskStatus Status,
    string RowVersion);
