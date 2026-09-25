namespace OpsFlow.Domain.Notifications;

public enum NotificationType
{
    TaskCreated = 1,
    ApprovalCreated = 2,
    ApprovalApproved = 3,
    ApprovalRejected = 4,
    MembershipInvited = 5
}

public sealed class Notification
{
    private Notification()
    {
    }

    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid RecipientUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Message { get; private set; }
    public string? ResourceType { get; private set; }
    public Guid? ResourceId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ReadAtUtc { get; private set; }

    public static Notification Create(
        Guid organizationId,
        Guid recipientUserId,
        NotificationType type,
        string title,
        string? message,
        string? resourceType,
        Guid? resourceId,
        DateTimeOffset createdAtUtc)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required.", nameof(organizationId));

        if (recipientUserId == Guid.Empty)
            throw new ArgumentException("Recipient user ID is required.", nameof(recipientUserId));

        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new Notification
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            RecipientUserId = recipientUserId,
            Type = type,
            Title = title.Trim(),
            Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
            ResourceType = string.IsNullOrWhiteSpace(resourceType) ? null : resourceType.Trim(),
            ResourceId = resourceId,
            IsRead = false,
            CreatedAtUtc = createdAtUtc
        };
    }

    public void MarkAsRead(DateTimeOffset readAtUtc)
    {
        if (IsRead)
            return;

        IsRead = true;
        ReadAtUtc = readAtUtc;
    }
}
