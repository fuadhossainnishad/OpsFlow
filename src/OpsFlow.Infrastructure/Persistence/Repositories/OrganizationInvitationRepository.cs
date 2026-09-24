using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class OrganizationInvitationRepository(
    OpsFlowDbContext dbContext) : IOrganizationInvitationRepository
{
    public Task<bool> HasPendingInvitationAsync(
        Guid organizationId,
        string normalizedEmail,
        CancellationToken cancellationToken)
    {
        return dbContext.OrganizationInvitations
            .AsNoTracking()
            .AnyAsync(
                invitation =>
                    invitation.OrganizationId == organizationId &&
                    invitation.NormalizedEmail == normalizedEmail &&
                    invitation.AcceptedAtUtc == null &&
                    invitation.RevokedAtUtc == null &&
                    invitation.ExpiresAtUtc > DateTimeOffset.UtcNow,
                cancellationToken);
    }

    public Task<OrganizationInvitation?> GetPendingByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return dbContext.OrganizationInvitations
            .SingleOrDefaultAsync(
                invitation =>
                    invitation.TokenHash == tokenHash &&
                    invitation.AcceptedAtUtc == null &&
                    invitation.RevokedAtUtc == null,
                cancellationToken);
    }

    public async Task AddAsync(
        OrganizationInvitation invitation,
        CancellationToken cancellationToken)
    {
        await dbContext.OrganizationInvitations.AddAsync(
            invitation,
            cancellationToken);
    }
}
