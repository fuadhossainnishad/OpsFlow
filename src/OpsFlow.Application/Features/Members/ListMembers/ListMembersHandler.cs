using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Members.ListMembers;

public sealed class ListMembersHandler(
    ITenantContext tenantContext,
    IMembershipRepository membershipRepository)
{
    public async Task<ListMembersResult> HandleAsync(
        ListMembersQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var records =
            await membershipRepository.GetOrganizationMembersAsync(
                organizationId,
                cancellationToken);

        var members = records
            .Select(record => new MemberResult(
                record.MembershipId,
                record.UserId,
                record.Email,
                record.FirstName,
                record.LastName,
                record.RoleId,
                record.RoleName,
                record.IsActive,
                record.JoinedAtUtc))
            .ToList();

        return new ListMembersResult(members);
    }
}
