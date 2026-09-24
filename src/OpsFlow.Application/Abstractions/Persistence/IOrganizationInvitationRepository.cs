using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IOrganizationInvitationRepository
{
    Task<bool> HasPendingInvitationAsync(
        Guid organizationId,
        string normalizedEmail,
        CancellationToken cancellationToken);

    Task<OrganizationInvitation?> GetPendingByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task AddAsync(
        OrganizationInvitation invitation,
        CancellationToken cancellationToken);
}
