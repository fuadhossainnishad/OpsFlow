using FluentAssertions;
using OpsFlow.Domain.Notifications;

namespace OpsFlow.UnitTests.Domain.Notifications;

public sealed class NotificationTests
{
    [Fact]
    public void CreateShouldInitializeNotification()
    {
        var organizationId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var resourceId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        var notification = Notification.Create(
            organizationId,
            recipientId,
            NotificationType.TaskCreated,
            "  New Task  ",
            "  A task was created.  ",
            "  Task  ",
            resourceId,
            createdAt);

        notification.Id.Should().NotBe(Guid.Empty);
        notification.OrganizationId.Should().Be(organizationId);
        notification.RecipientUserId.Should().Be(recipientId);
        notification.Type.Should().Be(NotificationType.TaskCreated);
        notification.Title.Should().Be("New Task");
        notification.Message.Should().Be("A task was created.");
        notification.ResourceType.Should().Be("Task");
        notification.ResourceId.Should().Be(resourceId);
        notification.IsRead.Should().BeFalse();
        notification.CreatedAtUtc.Should().Be(createdAt);
        notification.ReadAtUtc.Should().BeNull();
    }

    [Fact]
    public void CreateShouldNormalizeBlankOptionalValuesToNull()
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationType.TaskCreated,
            "Title",
            "   ",
            "   ",
            null,
            DateTimeOffset.UtcNow);

        notification.Message.Should().BeNull();
        notification.ResourceType.Should().BeNull();
        notification.ResourceId.Should().BeNull();
    }

    [Fact]
    public void CreateShouldAllowNullOptionalValues()
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationType.ApprovalCreated,
            "Approval requested",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        notification.Message.Should().BeNull();
        notification.ResourceType.Should().BeNull();
        notification.ResourceId.Should().BeNull();
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => Notification.Create(
            Guid.Empty,
            Guid.NewGuid(),
            NotificationType.TaskCreated,
            "Title",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Organization ID is required*");
    }

    [Fact]
    public void CreateShouldRejectEmptyRecipientId()
    {
        var action = () => Notification.Create(
            Guid.NewGuid(),
            Guid.Empty,
            NotificationType.TaskCreated,
            "Title",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Recipient user ID is required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankTitle(string title)
    {
        var action = () => Notification.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationType.TaskCreated,
            title,
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkAsReadShouldMarkUnreadNotification()
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationType.TaskCreated,
            "Task created",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        var readAt = DateTimeOffset.UtcNow;

        notification.MarkAsRead(readAt);

        notification.IsRead.Should().BeTrue();
        notification.ReadAtUtc.Should().Be(readAt);
    }

    [Fact]
    public void MarkAsReadShouldNotChangeAlreadyReadNotification()
    {
        var notification = Notification.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            NotificationType.TaskCreated,
            "Task created",
            null,
            null,
            null,
            DateTimeOffset.UtcNow);

        var firstReadAt = DateTimeOffset.UtcNow;
        var secondReadAt = firstReadAt.AddMinutes(10);

        notification.MarkAsRead(firstReadAt);
        notification.MarkAsRead(secondReadAt);

        notification.IsRead.Should().BeTrue();
        notification.ReadAtUtc.Should().Be(firstReadAt);
    }
}
