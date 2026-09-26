using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Contracts.Events;
using OpsFlow.Domain.Notifications;

namespace OpsFlow.Application.Features.Tasks.Events;

public static class TaskCreatedProcessor
{
    public static async Task ProcessAsync(
        TaskCreatedEvent message,
        INotificationRepository notificationRepository,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (!message.AssigneeUserId.HasValue)
            return;

        var notification = Notification.Create(
            message.OrganizationId,
            message.AssigneeUserId.Value,
            NotificationType.TaskCreated,
            "New task assigned",
            $"You were assigned the task \"{message.Title}\".",
            "task",
            message.TaskId,
            message.OccurredAt);

        await notificationRepository.AddAsync(
            notification,
            cancellationToken);
    }
}
