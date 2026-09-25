using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Notifications.ListNotifications;

public sealed class ListNotificationsHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    INotificationRepository repository)
{
    public async Task<ListNotificationsResult> HandleAsync(
        ListNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (currentUser.UserId == Guid.Empty)
            throw new UnauthorizedException("Authenticated user is required.");

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var items = await repository.ListAsync(
            organizationId,
            currentUser.UserId,
            query.UnreadOnly,
            (page - 1) * pageSize,
            pageSize,
            cancellationToken);

        return new ListNotificationsResult(
            items.Select(Map).ToArray(),
            page,
            pageSize);
    }

    private static NotificationItem Map(Domain.Notifications.Notification notification) =>
        new(
            notification.Id,
            notification.OrganizationId,
            notification.RecipientUserId,
            notification.Type,
            notification.Title,
            notification.Message,
            notification.ResourceType,
            notification.ResourceId,
            notification.IsRead,
            notification.CreatedAtUtc,
            notification.ReadAtUtc);
}
