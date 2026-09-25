using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Application.Features.Auditing.ListAuditLogs;

public sealed class ListAuditLogsHandler(
    ITenantContext tenantContext,
    IAuditLogRepository auditLogRepository)
{
    public async Task<ListAuditLogsResult> HandleAsync(
        ListAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var logs = await auditLogRepository.GetAsync(
            organizationId,
            query.Resource,
            query.ResourceId,
            page,
            pageSize,
            cancellationToken);

        return new ListAuditLogsResult(
            logs.Items.Select(audit => new AuditLogItemResult(
                audit.Id,
                audit.ActorUserId,
                audit.Action,
                audit.Resource,
                audit.ResourceId,
                audit.OccurredAtUtc,
                audit.IpAddress,
                audit.UserAgent,
                audit.CorrelationId,
                audit.BeforeJson,
                audit.AfterJson)).ToList(),
            page,
            pageSize,
            logs.HasNextPage);
    }
}
