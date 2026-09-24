namespace OpsFlow.Api.Contracts.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    string RowVersion);
