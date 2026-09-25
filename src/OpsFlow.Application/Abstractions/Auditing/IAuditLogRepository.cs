using OpsFlow.Domain.Auditing;

namespace OpsFlow.Application.Abstractions.Auditing;

public interface IAuditLogRepository
{
    Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AuditLog>> GetByResourceAsync(
        Guid organizationId,
        string resource,
        Guid resourceId,
        CancellationToken cancellationToken);
}
