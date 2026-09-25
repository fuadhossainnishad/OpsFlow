namespace OpsFlow.Application.Features.Approvals.RejectApproval;

public sealed record RejectApprovalCommand(
    Guid ApprovalId,
    string? Comment);
