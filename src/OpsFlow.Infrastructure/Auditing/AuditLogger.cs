using Microsoft.AspNetCore.Http;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Domain.Auditing;

namespace OpsFlow.Infrastructure.Auditing;

public sealed class AuditLogger(
    IAuditLogRepository repository,
    IHttpContextAccessor httpContextAccessor) : IAuditLogger
{
    public async Task LogAsync(
        Guid organizationId,
        Guid? actorUserId,
        string action,
        string resource,
        Guid? resourceId,
        string? beforeJson,
        string? afterJson,
        CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var correlationId = httpContext?.TraceIdentifier;
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

        var auditLog = AuditLog.Create(
            organizationId,
            actorUserId,
            action,
            resource,
            resourceId,
            ipAddress,
            userAgent,
            correlationId,
            beforeJson,
            afterJson);

        await repository.AddAsync(auditLog, cancellationToken);
    }
}
