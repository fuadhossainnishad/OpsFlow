using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Approvals;

namespace OpsFlow.Application.Features.Approvals.GetApproval;

public sealed class GetApprovalHandler(
    ITenantContext tenantContext,
    IApprovalRepository approvalRepository)
{
    public async Task<GetApprovalResult> HandleAsync(
        GetApprovalQuery query,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var approval = await approvalRepository.GetByIdAsync(
            organizationId,
            query.ApprovalId,
            cancellationToken);

        if (approval is null)
            throw new NotFoundException("Approval was not found.");

        return new GetApprovalResult(approval);
    }
}
