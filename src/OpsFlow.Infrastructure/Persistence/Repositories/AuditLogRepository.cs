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
        => await dbContext.AuditLogs.AddAsync(
            auditLog,
            cancellationToken);

    public async Task<AuditLogPage> GetAsync(
        Guid organizationId,
        string? resource,
        Guid? resourceId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.AuditLogs
            .AsNoTracking()
            .Where(audit => audit.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(resource))
        {
            var normalizedResource = resource.Trim();
            query = query.Where(audit => audit.Resource == normalizedResource);
        }

        if (resourceId.HasValue)
        {
            query = query.Where(
                audit => audit.ResourceId == resourceId.Value);
        }

        var items = await query
            .OrderByDescending(audit => audit.OccurredAtUtc)
            .ThenByDescending(audit => audit.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = items.Count > pageSize;

        if (hasNextPage)
        {
            items.RemoveAt(items.Count - 1);
        }

        return new AuditLogPage(items, hasNextPage);
    }
}
