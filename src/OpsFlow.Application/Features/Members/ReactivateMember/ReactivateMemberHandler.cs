using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Members.ReactivateMember;

public sealed class ReactivateMemberHandler(
    ITenantContext tenantContext,
    IMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ReactivateMemberResult> HandleAsync(
        ReactivateMemberCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var membership = await membershipRepository.GetByIdAsync(
            organizationId,
            command.MembershipId,
            cancellationToken);

        if (membership is null)
        {
            throw new NotFoundException("The membership was not found.");
        }

        membership.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReactivateMemberResult(
            membership.Id,
            membership.OrganizationId,
            membership.UserId,
            membership.IsActive);
    }
}
