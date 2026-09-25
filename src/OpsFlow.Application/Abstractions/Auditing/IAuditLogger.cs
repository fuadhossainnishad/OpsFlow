namespace OpsFlow.Application.Abstractions.Auditing;

public interface IAuditLogger
{
    Task LogAsync(
        Guid organizationId,
        Guid? actorUserId,
        string action,
        string resource,
        Guid? resourceId,
        string? beforeJson,
        string? afterJson,
        CancellationToken cancellationToken);
}
