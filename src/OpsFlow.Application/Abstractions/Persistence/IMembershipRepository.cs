using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IMembershipRepository
{
    Task AddAsync(
        Membership membership,
        CancellationToken cancellationToken);

    Task<Membership?> GetByIdAsync(
        Guid organizationId,
        Guid membershipId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<OrganizationMemberRecord>> GetOrganizationMembersAsync(
        Guid organizationId,
        CancellationToken cancellationToken);

    Task<bool> IsActiveMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken);
}

public sealed record OrganizationMemberRecord(
    Guid MembershipId,
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    Guid RoleId,
    string RoleName,
    bool IsActive,
    DateTimeOffset JoinedAtUtc);
