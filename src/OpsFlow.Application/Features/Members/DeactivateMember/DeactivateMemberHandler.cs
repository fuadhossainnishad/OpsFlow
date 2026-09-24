using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Members.DeactivateMember;

public sealed class DeactivateMemberHandler(
    ITenantContext tenantContext,
    IMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<DeactivateMemberResult> HandleAsync(
        DeactivateMemberCommand command,
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

        membership.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeactivateMemberResult(
            membership.Id,
            membership.OrganizationId,
            membership.UserId,
            membership.IsActive);
    }
}
