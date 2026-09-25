using OpsFlow.Domain.Auditing;

namespace OpsFlow.Application.Abstractions.Auditing;

public sealed record AuditLogPage(
    IReadOnlyList<AuditLog> Items,
    bool HasNextPage);
