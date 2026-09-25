using OpsFlow.Domain.Approvals;

namespace OpsFlow.Application.Features.Approvals.ListApprovals;

public sealed record ListApprovalsQuery(
    Guid? RequesterUserId,
    ApprovalStatus? Status);
