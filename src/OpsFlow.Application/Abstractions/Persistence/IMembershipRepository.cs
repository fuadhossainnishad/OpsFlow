using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface IMembershipRepository
{
    Task AddAsync(
        Membership membership,
        CancellationToken cancellationToken);
}
