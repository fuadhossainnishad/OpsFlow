using OpsFlow.Domain.Approvals;

namespace OpsFlow.Application.Features.Approvals;

public interface IApprovalRepository
{
    Task AddAsync(ApprovalRequest approval, CancellationToken cancellationToken);

    Task<ApprovalRequest?> GetByIdAsync(
        Guid organizationId,
        Guid approvalId,
        CancellationToken cancellationToken);

    Task<bool> HasPendingForTimeEntryAsync(
        Guid organizationId,
        Guid timeEntryId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ApprovalRequest>> ListAsync(
        Guid organizationId,
        Guid? requesterUserId,
        ApprovalStatus? status,
        CancellationToken cancellationToken);
}
