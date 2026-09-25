using OpsFlow.Domain.Notifications;

namespace OpsFlow.Application.Abstractions.Notifications;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken);

    Task<IReadOnlyList<Notification>> ListAsync(
        Guid organizationId,
        Guid recipientUserId,
        bool? unreadOnly,
        int skip,
        int take,
        CancellationToken cancellationToken);

    Task<int> GetUnreadCountAsync(
        Guid organizationId,
        Guid recipientUserId,
        CancellationToken cancellationToken);

    Task<Notification?> GetByIdAsync(
        Guid organizationId,
        Guid recipientUserId,
        Guid notificationId,
        CancellationToken cancellationToken);
}
