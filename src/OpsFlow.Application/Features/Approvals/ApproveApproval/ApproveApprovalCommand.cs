namespace OpsFlow.Application.Features.Approvals.ApproveApproval;

public sealed record ApproveApprovalCommand(
    Guid ApprovalId,
    string? Comment);
