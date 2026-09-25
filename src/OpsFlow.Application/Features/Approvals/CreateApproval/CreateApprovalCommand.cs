namespace OpsFlow.Application.Features.Approvals.CreateApproval;

public sealed record CreateApprovalCommand(
    Guid TimeEntryId,
    string? Comment);
