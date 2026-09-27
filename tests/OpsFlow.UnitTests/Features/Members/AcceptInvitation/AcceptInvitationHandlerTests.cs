using System.Text.Json;
using FluentAssertions;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Members.AcceptInvitation;
using OpsFlow.Domain.Identity;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.UnitTests.Features.Members.AcceptInvitation;

public sealed class AcceptInvitationHandlerTests
{
    [Fact]
    public async Task HandleShouldAcceptInvitationCreateMembershipAuditAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var user = User.Create(
            "fuad@example.com",
            "Fuad",
            "Hossain");

        var token = "valid-invitation-token";
        var invitation = OrganizationInvitation.Create(
            organizationId,
            user.Email,
            roleId,
            "token-hash",
            DateTimeOffset.UtcNow.AddHours(1));

        var membershipRepository = new FakeMembershipRepository();
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptInvitationHandler(
            new FakeCurrentUser(user.Id),
            new FakeUserRepository(user),
            new FakeInvitationRepository(invitation),
            membershipRepository,
            auditLogger,
            unitOfWork);

        var result = await handler.HandleAsync(
            new AcceptInvitationCommand(token),
            CancellationToken.None);

        result.MembershipId.Should().NotBeEmpty();
        result.OrganizationId.Should().Be(organizationId);
        result.UserId.Should().Be(user.Id);
        result.RoleId.Should().Be(roleId);

        invitation.AcceptedAtUtc.Should().NotBeNull();

        membershipRepository.AddCallCount.Should().Be(1);
        membershipRepository.AddedMembership.Should().NotBeNull();
        membershipRepository.AddedMembership!.OrganizationId
            .Should().Be(organizationId);
        membershipRepository.AddedMembership.UserId
            .Should().Be(user.Id);
        membershipRepository.AddedMembership.RoleId
            .Should().Be(roleId);

        auditLogger.CallCount.Should().Be(1);
        auditLogger.OrganizationId.Should().Be(organizationId);
        auditLogger.ActorUserId.Should().Be(user.Id);
        auditLogger.Action.Should().Be("membership.invitation_accepted");
        auditLogger.Resource.Should().Be("organization_invitation");
        auditLogger.ResourceId.Should().Be(invitation.Id);
        auditLogger.BeforeJson.Should().BeNull();
        auditLogger.AfterJson.Should().NotBeNull();
        auditLogger.AfterJson.Should().Contain(invitation.Id.ToString());
        auditLogger.AfterJson.Should().Contain(
            membershipRepository.AddedMembership.Id.ToString());
        auditLogger.AfterJson.Should().Contain(user.Id.ToString());
        auditLogger.AfterJson.Should().Contain(roleId.ToString());

        unitOfWork.SaveChangesCallCount.Should().Be(1);
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

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleShouldRejectBlankToken(string token)
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand(token),
            CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task HandleShouldRejectWhenInvitationDoesNotExist()
    {
        var handler = CreateHandler(
            invitation: null);

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand("missing-token"),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should().Be("Invitation was not found.");
    }

    [Fact]
    public async Task HandleShouldRejectWhenInvitationIsExpired()
    {
        var organizationId = Guid.NewGuid();
        var user = User.Create(
            "fuad@example.com",
            "Fuad",
            "Hossain");

        var invitation = CreateExpiredInvitation(
            organizationId,
            user.Email,
            Guid.NewGuid());

        var membershipRepository = new FakeMembershipRepository();
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptInvitationHandler(
            new FakeCurrentUser(user.Id),
            new FakeUserRepository(user),
            new FakeInvitationRepository(invitation),
            membershipRepository,
            auditLogger,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand("expired-token"),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should().Be("The invitation has expired.");

        membershipRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenUserDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var invitation = OrganizationInvitation.Create(
            organizationId,
            "missing@example.com",
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddHours(1));

        var membershipRepository = new FakeMembershipRepository();
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptInvitationHandler(
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeUserRepository(null),
            new FakeInvitationRepository(invitation),
            membershipRepository,
            auditLogger,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand("valid-token"),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should().Be(
                "The invited email does not belong to an active user.");

        membershipRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenUserIsInactive()
    {
        var organizationId = Guid.NewGuid();
        var user = User.Create(
            "inactive@example.com",
            "Inactive",
            "User");

        user.Deactivate();

        var invitation = OrganizationInvitation.Create(
            organizationId,
            user.Email,
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddHours(1));

        var membershipRepository = new FakeMembershipRepository();
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptInvitationHandler(
            new FakeCurrentUser(user.Id),
            new FakeUserRepository(user),
            new FakeInvitationRepository(invitation),
            membershipRepository,
            auditLogger,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand("valid-token"),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should().Be(
                "The invited email does not belong to an active user.");

        membershipRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenInvitationBelongsToDifferentUser()
    {
        var organizationId = Guid.NewGuid();
        var invitedUser = User.Create(
            "invited@example.com",
            "Invited",
            "User");

        var currentUserId = Guid.NewGuid();

        var invitation = OrganizationInvitation.Create(
            organizationId,
            invitedUser.Email,
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddHours(1));

        var membershipRepository = new FakeMembershipRepository();
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptInvitationHandler(
            new FakeCurrentUser(currentUserId),
            new FakeUserRepository(invitedUser),
            new FakeInvitationRepository(invitation),
            membershipRepository,
            auditLogger,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand("valid-token"),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ForbiddenException>();

        exception.Which.Message
            .Should().Be(
                "The invitation belongs to a different user.");

        membershipRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenUserIsAlreadyActiveMember()
    {
        var organizationId = Guid.NewGuid();
        var user = User.Create(
            "member@example.com",
            "Existing",
            "Member");

        var invitation = OrganizationInvitation.Create(
            organizationId,
            user.Email,
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddHours(1));

        var membershipRepository = new FakeMembershipRepository
        {
            IsActiveMember = true
        };

        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new AcceptInvitationHandler(
            new FakeCurrentUser(user.Id),
            new FakeUserRepository(user),
            new FakeInvitationRepository(invitation),
            membershipRepository,
            auditLogger,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new AcceptInvitationCommand("valid-token"),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should().Be(
                "The user is already an active member of the organization.");

        invitation.AcceptedAtUtc.Should().BeNull();
        membershipRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static AcceptInvitationHandler CreateHandler(
        OrganizationInvitation? invitation = null)
    {
        return new AcceptInvitationHandler(
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeUserRepository(null),
            new FakeInvitationRepository(invitation),
            new FakeMembershipRepository(),
            new FakeAuditLogger(),
            new FakeUnitOfWork());
    }

    private static OrganizationInvitation CreateExpiredInvitation(
        Guid organizationId,
        string email,
        Guid roleId)
    {
        var constructor = typeof(OrganizationInvitation)
            .GetConstructor(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                binder: null,
                new[]
                {
                    typeof(Guid),
                    typeof(Guid),
                    typeof(string),
                    typeof(string),
                    typeof(Guid),
                    typeof(string),
                    typeof(DateTimeOffset)
                },
                modifiers: null);

        constructor.Should().NotBeNull();

        return (OrganizationInvitation)constructor!.Invoke(
        [
            Guid.NewGuid(),
            organizationId,
            email,
            email.ToUpperInvariant(),
            roleId,
            "expired-token-hash",
            DateTimeOffset.UtcNow.AddMinutes(-1)
        ]);
    }

    private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeUserRepository(User? user) : IUserRepository
    {
        public Task<User?> GetByNormalizedEmailAsync(
            string normalizedEmail,
            CancellationToken cancellationToken)
            => Task.FromResult(user);

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private sealed class FakeInvitationRepository(
        OrganizationInvitation? invitation)
        : IOrganizationInvitationRepository
    {
        public Task<bool> HasPendingInvitationAsync(
            Guid organizationId,
            string normalizedEmail,
            CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<OrganizationInvitation?> GetPendingByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken)
            => Task.FromResult(invitation);

        public Task AddAsync(
            OrganizationInvitation invitation,
            CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private sealed class FakeMembershipRepository : IMembershipRepository
    {
        public bool IsActiveMember { get; init; }
        public int AddCallCount { get; private set; }
        public Membership? AddedMembership { get; private set; }

        public Task AddAsync(
            Membership membership,
            CancellationToken cancellationToken)
        {
            AddCallCount++;
            AddedMembership = membership;
            return Task.CompletedTask;
        }

        public Task<Membership?> GetByIdAsync(
            Guid organizationId,
            Guid membershipId,
            CancellationToken cancellationToken)
            => Task.FromResult<Membership?>(null);

        public Task<IReadOnlyList<OrganizationMemberRecord>>
            GetOrganizationMembersAsync(
                Guid organizationId,
                CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<OrganizationMemberRecord>>([]);

        public Task<bool> IsActiveMemberAsync(
            Guid organizationId,
            Guid userId,
            CancellationToken cancellationToken)
            => Task.FromResult(IsActiveMember);
    }

    private sealed class FakeAuditLogger : IAuditLogger
    {
        public int CallCount { get; private set; }
        public Guid OrganizationId { get; private set; }
        public Guid? ActorUserId { get; private set; }
        public string? Action { get; private set; }
        public string? Resource { get; private set; }
        public Guid? ResourceId { get; private set; }
        public string? BeforeJson { get; private set; }
        public string? AfterJson { get; private set; }

        public Task LogAsync(
            Guid organizationId,
            Guid? actorUserId,
            string action,
            string resource,
            Guid? resourceId,
            string? beforeJson,
            string? afterJson,
            CancellationToken cancellationToken)
        {
            CallCount++;
            OrganizationId = organizationId;
            ActorUserId = actorUserId;
            Action = action;
            Resource = resource;
            ResourceId = resourceId;
            BeforeJson = beforeJson;
            AfterJson = afterJson;

            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
