using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Features.Approvals;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class ApprovalRepository(
    OpsFlowDbContext dbContext) : IApprovalRepository
{
    public async Task AddAsync(
        ApprovalRequest approval,
        CancellationToken cancellationToken)
        => await dbContext.ApprovalRequests.AddAsync(
            approval,
            cancellationToken);

    public Task<ApprovalRequest?> GetByIdAsync(
        Guid organizationId,
        Guid approvalId,
        CancellationToken cancellationToken)
        => dbContext.ApprovalRequests.SingleOrDefaultAsync(
            x => x.OrganizationId == organizationId && x.Id == approvalId,
            cancellationToken);

    public Task<bool> HasPendingForTimeEntryAsync(
        Guid organizationId,
        Guid timeEntryId,
        CancellationToken cancellationToken)
        => dbContext.ApprovalRequests.AnyAsync(
            x =>
                x.OrganizationId == organizationId &&
                x.TimeEntryId == timeEntryId &&
                x.Status == ApprovalStatus.Pending,
            cancellationToken);

    public async Task<IReadOnlyList<ApprovalRequest>> ListAsync(
        Guid organizationId,
        Guid? requesterUserId,
        ApprovalStatus? status,
        CancellationToken cancellationToken)
        => await dbContext.ApprovalRequests
            .AsNoTracking()
            .Where(x =>
                x.OrganizationId == organizationId &&
                (!requesterUserId.HasValue ||
                 x.RequesterUserId == requesterUserId.Value) &&
                (!status.HasValue || x.Status == status.Value))
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
}
