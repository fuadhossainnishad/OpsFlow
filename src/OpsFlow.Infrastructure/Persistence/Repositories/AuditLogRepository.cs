using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Domain.Auditing;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(
    OpsFlowDbContext dbContext) : IAuditLogRepository
{
    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken)
    {
        await dbContext.AuditLogs.AddAsync(
            auditLog,
            cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByResourceAsync(
        Guid organizationId,
        string resource,
        Guid resourceId,
        CancellationToken cancellationToken)
        => await dbContext.AuditLogs
            .AsNoTracking()
            .Where(audit =>
                audit.OrganizationId == organizationId &&
                audit.Resource == resource &&
                audit.ResourceId == resourceId)
            .OrderByDescending(audit => audit.OccurredAtUtc)
            .ToListAsync(cancellationToken);
}
