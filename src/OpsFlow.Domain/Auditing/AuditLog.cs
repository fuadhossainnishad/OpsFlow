using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Auditing;

public sealed class AuditLog : Entity
{
    private AuditLog()
    {
    }

    private AuditLog(
        Guid id,
        Guid organizationId,
        Guid? actorUserId,
        string action,
        string resource,
        Guid? resourceId,
        DateTimeOffset occurredAtUtc,
        string? ipAddress,
        string? userAgent,
        string? correlationId,
        string? beforeJson,
        string? afterJson)
    {
        Id = id;
        OrganizationId = organizationId;
        ActorUserId = actorUserId;
        Action = action;
        Resource = resource;
        ResourceId = resourceId;
        OccurredAtUtc = occurredAtUtc;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CorrelationId = correlationId;
        BeforeJson = beforeJson;
        AfterJson = afterJson;
    }

    public Guid OrganizationId { get; private init; }

    public Guid? ActorUserId { get; private init; }

    public string Action { get; private init; } = null!;

    public string Resource { get; private init; } = null!;

    public Guid? ResourceId { get; private init; }

    public DateTimeOffset OccurredAtUtc { get; private init; }

    public string? IpAddress { get; private init; }

    public string? UserAgent { get; private init; }

    public string? CorrelationId { get; private init; }

    public string? BeforeJson { get; private init; }

    public string? AfterJson { get; private init; }

    public static AuditLog Create(
        Guid organizationId,
        Guid? actorUserId,
        string action,
        string resource,
        Guid? resourceId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? correlationId = null,
        string? beforeJson = null,
        string? afterJson = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        if (actorUserId == Guid.Empty)
        {
            actorUserId = null;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(action);
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);

        return new AuditLog(
            Guid.NewGuid(),
            organizationId,
            actorUserId,
            action.Trim(),
            resource.Trim(),
            resourceId,
            DateTimeOffset.UtcNow,
            ipAddress?.Trim(),
            userAgent?.Trim(),
            correlationId?.Trim(),
            beforeJson,
            afterJson);
    }
}
