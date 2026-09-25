using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Approvals;

namespace OpsFlow.Application.Features.Approvals.RejectApproval;

public sealed class RejectApprovalHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IApprovalRepository approvalRepository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        RejectApprovalCommand command,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var approval = await approvalRepository.GetByIdAsync(
            organizationId,
            command.ApprovalId,
            cancellationToken);

        if (approval is null)
            throw new NotFoundException("Approval was not found.");

        try
        {
            approval.Reject(currentUser.UserId, command.Comment);
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
