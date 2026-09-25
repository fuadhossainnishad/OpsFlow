namespace OpsFlow.Api.Contracts.Approvals;

public sealed record CreateApprovalRequest(
    Guid TimeEntryId,
    string? Comment);
