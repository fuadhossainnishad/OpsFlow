namespace OpsFlow.Api.Contracts.Tasks;

public sealed record AssignTaskRequest(
    Guid AssigneeUserId,
    string RowVersion);
