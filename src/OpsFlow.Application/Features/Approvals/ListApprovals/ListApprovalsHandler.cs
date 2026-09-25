using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Approvals;

namespace OpsFlow.Application.Features.Approvals.ListApprovals;

public sealed class ListApprovalsHandler(
    ITenantContext tenantContext,
    IApprovalRepository approvalRepository)
{
    public async Task<ListApprovalsResult> HandleAsync(
        ListApprovalsQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var approvals = await approvalRepository.ListAsync(
            organizationId,
            query.RequesterUserId,
            query.Status,
            cancellationToken);

        return new ListApprovalsResult(approvals);
    }
}
