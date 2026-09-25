using OpsFlow.Domain.Approvals;

namespace OpsFlow.Application.Features.Approvals.ListApprovals;

public sealed record ListApprovalsResult(
    IReadOnlyList<ApprovalRequest> Approvals);
