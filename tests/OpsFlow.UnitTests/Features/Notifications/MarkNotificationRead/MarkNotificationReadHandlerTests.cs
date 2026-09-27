using FluentAssertions;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Notifications.MarkNotificationRead;
using OpsFlow.Domain.Notifications;

namespace OpsFlow.UnitTests.Features.Notifications.MarkNotificationRead;

public sealed class MarkNotificationReadHandlerTests
{
    private static readonly Guid OrganizationId =
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid UserId =
        Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleShouldMarkNotificationAsReadAndSave()
    {
        var notification = Notification.Create(
            OrganizationId,
            UserId,
            NotificationType.MembershipInvited,
            "Task assigned",
            "You were assigned a task.",
            "task",
            Guid.NewGuid(),
            DateTimeOffset.UtcNow.AddMinutes(-5));

        var repository = new FakeNotificationRepository
        {
            Notification = notification
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = CreateHandler(
            repository,
            unitOfWork);

        notification.IsRead.Should().BeFalse();
        notification.ReadAtUtc.Should().BeNull();

        await handler.HandleAsync(
            new MarkNotificationReadCommand(notification.Id),
            CancellationToken.None);

        notification.IsRead.Should().BeTrue();
        notification.ReadAtUtc.Should().NotBeNull();

        repository.RequestedOrganizationId.Should().Be(OrganizationId);
        repository.RequestedUserId.Should().Be(UserId);
        repository.RequestedNotificationId.Should().Be(notification.Id);

        unitOfWork.SaveCalls.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectNullCommand()
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            null!,
            CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task HandleShouldRejectMissingAuthenticatedUser()
    {
        var handler = CreateHandler(
            currentUser: new FakeCurrentUser(Guid.Empty));

        var action = () => handler.HandleAsync(
            new MarkNotificationReadCommand(Guid.NewGuid()),
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<UnauthorizedException>()
            .WithMessage("*Authenticated user is required*");
    }

    [Fact]
    public async Task HandleShouldRejectWhenNotificationDoesNotExist()
    {
        var repository = new FakeNotificationRepository();

        var handler = CreateHandler(repository);

        var notificationId = Guid.NewGuid();

        var action = () => handler.HandleAsync(
            new MarkNotificationReadCommand(notificationId),
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*Notification was not found*");

        repository.RequestedOrganizationId.Should().Be(OrganizationId);
        repository.RequestedUserId.Should().Be(UserId);
        repository.RequestedNotificationId.Should().Be(notificationId);
    }

    private static MarkNotificationReadHandler CreateHandler(
        FakeNotificationRepository? repository = null,
        FakeUnitOfWork? unitOfWork = null,
        FakeCurrentUser? currentUser = null)
    {
        return new MarkNotificationReadHandler(
            new FakeTenantContext(),
            currentUser ?? new FakeCurrentUser(UserId),
            repository ?? new FakeNotificationRepository(),
            unitOfWork ?? new FakeUnitOfWork());
    }

    private sealed class FakeNotificationRepository : INotificationRepository
    {
        public Notification? Notification { get; init; }

        public Guid RequestedOrganizationId { get; private set; }
        public Guid RequestedUserId { get; private set; }
        public Guid RequestedNotificationId { get; private set; }

        public Task AddAsync(
            Notification notification,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task<IReadOnlyList<Notification>> ListAsync(
            Guid organizationId,
            Guid recipientUserId,
            bool? unreadOnly,
            int skip,
            int take,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<Notification>>([]);

        public Task<int> GetUnreadCountAsync(
            Guid organizationId,
            Guid recipientUserId,
            CancellationToken cancellationToken) =>
            Task.FromResult(0);

        public Task<Notification?> GetByIdAsync(
            Guid organizationId,
            Guid recipientUserId,
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            RequestedOrganizationId = organizationId;
            RequestedUserId = recipientUserId;
            RequestedNotificationId = notificationId;

            return Task.FromResult(Notification);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCalls { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveCalls++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeTenantContext : ITenantContext
    {
        public Task<Guid> GetOrganizationIdAsync(
            CancellationToken cancellationToken) =>
            Task.FromResult(OrganizationId);
    }
}
