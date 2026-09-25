using OpsFlow.Domain.Auditing;

namespace OpsFlow.Application.Abstractions.Auditing;

public interface IAuditLogRepository
{
    Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken);

    Task<AuditLogPage> GetAsync(
        Guid organizationId,
        string? resource,
        Guid? resourceId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
