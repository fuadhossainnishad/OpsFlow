namespace OpsFlow.Application.Features.Auditing.ListAuditLogs;

public sealed record AuditLogItemResult(
    Guid Id,
    Guid? ActorUserId,
    string Action,
    string Resource,
    Guid? ResourceId,
    DateTimeOffset OccurredAtUtc,
    string? IpAddress,
    string? UserAgent,
    string? CorrelationId,
    string? BeforeJson,
    string? AfterJson);

public sealed record ListAuditLogsResult(
    IReadOnlyList<AuditLogItemResult> Items,
    int Page,
    int PageSize,
    bool HasNextPage);
