using FluentAssertions;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Application.Features.Tasks.Events;
using OpsFlow.Contracts.Events;
using OpsFlow.Domain.Notifications;

namespace OpsFlow.UnitTests.Features.Tasks.Events;

public sealed class TaskCreatedProcessorTests
{
    [Fact]
    public async Task ProcessAsyncShouldCreateNotificationForAssignee()
    {
        var organizationId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var assigneeUserId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.UtcNow;

        var repository = new FakeNotificationRepository();

        var message = new TaskCreatedEvent(
            taskId,
            organizationId,
            projectId,
            "Prepare monthly report",
            assigneeUserId,
            occurredAt);

        await TaskCreatedProcessor.ProcessAsync(
            message,
            repository,
            CancellationToken.None);

        repository.AddedNotification.Should().NotBeNull();

        var notification = repository.AddedNotification!;

        notification.OrganizationId.Should().Be(organizationId);
        notification.RecipientUserId.Should().Be(assigneeUserId);
        notification.Type.Should().Be(NotificationType.TaskCreated);
        notification.Title.Should().Be("New task assigned");
        notification.Message.Should()
            .Be("You were assigned the task \"Prepare monthly report\".");
        notification.ResourceType.Should().Be("task");
        notification.ResourceId.Should().Be(taskId);
        notification.CreatedAtUtc.Should().Be(occurredAt);
        notification.IsRead.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessAsyncShouldNotCreateNotificationWhenTaskIsUnassigned()
    {
        var repository = new FakeNotificationRepository();

        var message = new TaskCreatedEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Unassigned task",
            null,
            DateTimeOffset.UtcNow);

        await TaskCreatedProcessor.ProcessAsync(
            message,
            repository,
            CancellationToken.None);

        repository.AddedNotification.Should().BeNull();
    }

    [Fact]
    public async Task ProcessAsyncShouldRejectNullMessage()
    {
        var repository = new FakeNotificationRepository();

        var action = () => TaskCreatedProcessor.ProcessAsync(
            null!,
            repository,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentNullException>();
    }

    private sealed class FakeNotificationRepository
        : INotificationRepository
    {
        public Notification? AddedNotification { get; private set; }

        public Task AddAsync(
            Notification notification,
            CancellationToken cancellationToken)
        {
            AddedNotification = notification;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<Notification>> ListAsync(
            Guid organizationId,
            Guid recipientUserId,
            bool? unreadOnly,
            int skip,
            int take,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Notification>>([]);
        }

        public Task<int> GetUnreadCountAsync(
            Guid organizationId,
            Guid recipientUserId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task<Notification?> GetByIdAsync(
            Guid organizationId,
            Guid recipientUserId,
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<Notification?>(null);
        }
    }
}
