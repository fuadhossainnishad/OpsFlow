using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Members.ChangeMemberRole;

public sealed class ChangeMemberRoleHandler(
    ITenantContext tenantContext,
    IMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ChangeMemberRoleResult> HandleAsync(
        ChangeMemberRoleCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.RoleName);

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

        var normalizedRoleName =
            command.RoleName.Trim().ToUpperInvariant();

        var role = await roleRepository.GetByNormalizedNameAsync(
            normalizedRoleName,
            cancellationToken);

        if (role is null || !role.IsSystemRole)
        {
            throw new NotFoundException("The requested role was not found.");
        }

        membership.ChangeRole(role.Id);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangeMemberRoleResult(
            membership.Id,
            membership.OrganizationId,
            membership.UserId,
            role.Id,
            role.Name,
            membership.IsActive);
    }
}
