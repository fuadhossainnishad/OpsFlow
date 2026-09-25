using OpsFlow.Domain.Notifications;

namespace OpsFlow.Application.Features.Notifications.ListNotifications;

public sealed record NotificationItem(
    Guid Id,
    Guid OrganizationId,
    Guid RecipientUserId,
    NotificationType Type,
    string Title,
    string? Message,
    string? ResourceType,
    Guid? ResourceId,
    bool IsRead,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ReadAtUtc);

public sealed record ListNotificationsResult(
    IReadOnlyList<NotificationItem> Items,
    int Page,
    int PageSize);
