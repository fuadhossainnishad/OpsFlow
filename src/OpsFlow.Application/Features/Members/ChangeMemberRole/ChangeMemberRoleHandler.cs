using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Members.ChangeMemberRole;

public sealed class ChangeMemberRoleHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    IAuditLogger auditLogger,
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

        var normalizedRoleName = command.RoleName
            .Trim()
            .ToUpperInvariant()
            .Replace(' ', '_');

        var role = await roleRepository.GetByNormalizedNameAsync(
            normalizedRoleName,
            cancellationToken);

        if (role is null || !role.IsSystemRole)
        {
            throw new NotFoundException("The requested role was not found.");
        }

        var beforeJson = JsonSerializer.Serialize(new
        {
            membership.Id,
            membership.UserId,
            membership.RoleId,
            membership.IsActive
        });

        membership.ChangeRole(role.Id);

        var afterJson = JsonSerializer.Serialize(new
        {
            membership.Id,
            membership.UserId,
            membership.RoleId,
            Role = role.Name,
            membership.IsActive
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "membership.role_changed",
            "membership",
            membership.Id,
            beforeJson,
            afterJson,
            cancellationToken);

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
