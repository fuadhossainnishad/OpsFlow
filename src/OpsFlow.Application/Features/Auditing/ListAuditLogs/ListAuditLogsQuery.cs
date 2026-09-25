namespace OpsFlow.Application.Features.Auditing.ListAuditLogs;

public sealed record ListAuditLogsQuery(
    int Page = 1,
    int PageSize = 50,
    string? Resource = null,
    Guid? ResourceId = null);
