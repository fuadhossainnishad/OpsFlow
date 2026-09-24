namespace OpsFlow.Application.Abstractions.Tenancy;

public interface ITenantContext
{
    Task<Guid> GetOrganizationIdAsync(CancellationToken cancellationToken);
}
