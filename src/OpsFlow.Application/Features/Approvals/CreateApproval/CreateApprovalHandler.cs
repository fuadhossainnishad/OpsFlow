using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.TimeTracking;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.Application.Features.Approvals.CreateApproval;

public sealed class CreateApprovalHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    ITimeEntryRepository timeEntryRepository,
    IApprovalRepository approvalRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateApprovalResult> HandleAsync(
        CreateApprovalCommand command,
        CancellationToken cancellationToken)
    {
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        if (currentUser.UserId == Guid.Empty)
            throw new UnauthorizedException("Authenticated user is required.");

        var entry = await timeEntryRepository.GetByIdAsync(
            organizationId,
            currentUser.UserId,
            command.TimeEntryId,
            cancellationToken);

        if (entry is null)
            throw new NotFoundException("Time entry was not found.");

        if (entry.IsRunning)
            throw new ConflictException(
                "A running time entry cannot be submitted for approval.");

        if (await approvalRepository.HasPendingForTimeEntryAsync(
                organizationId,
                command.TimeEntryId,
                cancellationToken))
        {
            throw new ConflictException(
                "The time entry already has a pending approval.");
        }

        var approval = ApprovalRequest.Create(
            organizationId,
            currentUser.UserId,
            command.TimeEntryId,
            command.Comment);

        await approvalRepository.AddAsync(approval, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateApprovalResult(
            approval.Id,
            approval.OrganizationId,
            approval.RequesterUserId,
            approval.TimeEntryId,
            approval.Comment,
            approval.Status,
            approval.CreatedAtUtc);
    }
}
