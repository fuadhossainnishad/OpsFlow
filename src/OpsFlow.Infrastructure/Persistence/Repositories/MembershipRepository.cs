using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class MembershipRepository(
    OpsFlowDbContext dbContext) : IMembershipRepository
{
    public async Task AddAsync(
        Membership membership,
        CancellationToken cancellationToken)
    {
        await dbContext.Memberships.AddAsync(
            membership,
            cancellationToken);
    }

    public Task<Membership?> GetByIdAsync(
        Guid organizationId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        return dbContext.Memberships
            .SingleOrDefaultAsync(
                membership =>
                    membership.OrganizationId == organizationId &&
                    membership.Id == membershipId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<OrganizationMemberRecord>> GetOrganizationMembersAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        return await (
            from membership in dbContext.Memberships.AsNoTracking()
            join user in dbContext.Users.AsNoTracking()
                on membership.UserId equals user.Id
            join role in dbContext.Roles.AsNoTracking()
                on membership.RoleId equals role.Id
            where membership.OrganizationId == organizationId
            orderby membership.JoinedAtUtc
            select new OrganizationMemberRecord(
                membership.Id,
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                role.Id,
                role.Name,
                membership.IsActive,
                membership.JoinedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsActiveMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Memberships
            .AsNoTracking()
            .AnyAsync(
                membership =>
                    membership.OrganizationId == organizationId &&
                    membership.UserId == userId &&
                    membership.IsActive,
                cancellationToken);
    }
}
