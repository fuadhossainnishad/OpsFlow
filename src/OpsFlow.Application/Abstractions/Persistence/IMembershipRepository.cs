using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IMembershipRepository
{
    Task AddAsync(
        Membership membership,
        CancellationToken cancellationToken);

    Task<bool> IsActiveMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken);
}
