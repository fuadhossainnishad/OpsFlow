using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Members.DeactivateMember;

public sealed class DeactivateMemberHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IMembershipRepository membershipRepository,
    IAuditLogger auditLogger,
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

        var beforeJson = JsonSerializer.Serialize(new
        {
            membership.Id,
            membership.UserId,
            membership.IsActive
        });

        membership.Deactivate();

        var afterJson = JsonSerializer.Serialize(new
        {
            membership.Id,
            membership.UserId,
            membership.IsActive
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "membership.deactivated",
            "membership",
            membership.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeactivateMemberResult(
            membership.Id,
            membership.OrganizationId,
            membership.UserId,
            membership.IsActive);
    }
}
