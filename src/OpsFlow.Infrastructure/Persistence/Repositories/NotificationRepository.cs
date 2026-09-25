using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Domain.Notifications;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(OpsFlowDbContext dbContext)
    : INotificationRepository
{
    public async Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken)
    {
        await dbContext.Notifications.AddAsync(notification, cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> ListAsync(
        Guid organizationId,
        Guid recipientUserId,
        bool? unreadOnly,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Notifications
            .AsNoTracking()
            .Where(x =>
                x.OrganizationId == organizationId &&
                x.RecipientUserId == recipientUserId);

        if (unreadOnly == true)
            query = query.Where(x => !x.IsRead);

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<int> GetUnreadCountAsync(
        Guid organizationId,
        Guid recipientUserId,
        CancellationToken cancellationToken) =>
        dbContext.Notifications.CountAsync(
            x =>
                x.OrganizationId == organizationId &&
                x.RecipientUserId == recipientUserId &&
                !x.IsRead,
            cancellationToken);

    public Task<Notification?> GetByIdAsync(
        Guid organizationId,
        Guid recipientUserId,
        Guid notificationId,
        CancellationToken cancellationToken) =>
        dbContext.Notifications.SingleOrDefaultAsync(
            x =>
                x.Id == notificationId &&
                x.OrganizationId == organizationId &&
                x.RecipientUserId == recipientUserId,
            cancellationToken);
}
