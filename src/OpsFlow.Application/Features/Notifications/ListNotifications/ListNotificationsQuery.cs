namespace OpsFlow.Application.Features.Notifications.ListNotifications;

public sealed record ListNotificationsQuery(
    bool? UnreadOnly,
    int Page = 1,
    int PageSize = 20);
