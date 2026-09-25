using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Members.ReactivateMember;

public sealed class ReactivateMemberHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    IMembershipRepository membershipRepository,
    IAuditLogger auditLogger,
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

        var beforeJson = JsonSerializer.Serialize(new
        {
            membership.Id,
            membership.UserId,
            membership.IsActive
        });

        membership.Activate();

        var afterJson = JsonSerializer.Serialize(new
        {
            membership.Id,
            membership.UserId,
            membership.IsActive
        });

        await auditLogger.LogAsync(
            organizationId,
            currentUser.UserId,
            "membership.reactivated",
            "membership",
            membership.Id,
            beforeJson,
            afterJson,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReactivateMemberResult(
            membership.Id,
            membership.OrganizationId,
            membership.UserId,
            membership.IsActive);
    }
}
