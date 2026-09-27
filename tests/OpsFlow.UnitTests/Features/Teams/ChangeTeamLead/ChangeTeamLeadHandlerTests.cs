using FluentAssertions;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Teams;
using OpsFlow.Application.Features.Teams.ChangeTeamLead;
using OpsFlow.Domain.Organizations;
using OpsFlow.Domain.Teams;

namespace OpsFlow.UnitTests.Features.Teams.ChangeTeamLead;

public sealed class ChangeTeamLeadHandlerTests
{
    [Fact]
    public async Task HandleShouldChangeTeamLeadAuditAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Engineering",
            "Engineering team");

        var membership = Membership.Create(
            organizationId,
            Guid.NewGuid(),
            Guid.NewGuid());

        var teamRepository = new FakeTeamRepository
        {
            Team = team,
            IsTeamMember = true
        };

        var membershipRepository = new FakeMembershipRepository
        {
            Membership = membership
        };

        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ChangeTeamLeadHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(currentUserId),
            teamRepository,
            membershipRepository,
            auditLogger,
            unitOfWork);

        var result = await handler.Handle(
            new ChangeTeamLeadCommand(team.Id, membership.Id),
            CancellationToken.None);

        result.TeamId.Should().Be(team.Id);
        result.TeamLeadMembershipId.Should().Be(membership.Id);

        team.TeamLeadMembershipId.Should().Be(membership.Id);
        team.UpdatedAtUtc.Should().NotBeNull();

        teamRepository.IsTeamMemberCallCount.Should().Be(1);
        auditLogger.CallCount.Should().Be(1);
        auditLogger.OrganizationId.Should().Be(organizationId);
        auditLogger.ActorUserId.Should().Be(currentUserId);
        auditLogger.Action.Should().Be("team.lead_changed");
        auditLogger.Resource.Should().Be("team");
        auditLogger.ResourceId.Should().Be(team.Id);
        auditLogger.BeforeJson.Should().Contain(team.Id.ToString());
        auditLogger.BeforeJson.Should().Contain("\"TeamLeadMembershipId\":null");
        auditLogger.AfterJson.Should().Contain(team.Id.ToString());
        auditLogger.AfterJson.Should().Contain(membership.Id.ToString());

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectWhenTeamDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork();
        var auditLogger = new FakeAuditLogger();

        var handler = new ChangeTeamLeadHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeTeamRepository(),
            new FakeMembershipRepository(),
            auditLogger,
            unitOfWork);

        var action = () => handler.Handle(
            new ChangeTeamLeadCommand(Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Team was not found.");

        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenTeamIsArchived()
    {
        var organizationId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            null);

        team.Archive();

        var unitOfWork = new FakeUnitOfWork();
        var auditLogger = new FakeAuditLogger();

        var handler = new ChangeTeamLeadHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeTeamRepository
            {
                Team = team
            },
            new FakeMembershipRepository(),
            auditLogger,
            unitOfWork);

        var action = () => handler.Handle(
            new ChangeTeamLeadCommand(team.Id, Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("Archived teams cannot have their lead changed.");

        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenMembershipDoesNotExist()
    {
        var organizationId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            null);

        var unitOfWork = new FakeUnitOfWork();
        var auditLogger = new FakeAuditLogger();

        var handler = new ChangeTeamLeadHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeTeamRepository
            {
                Team = team
            },
            new FakeMembershipRepository(),
            auditLogger,
            unitOfWork);

        var action = () => handler.Handle(
            new ChangeTeamLeadCommand(team.Id, Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Active organization membership was not found.");

        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenMembershipIsInactive()
    {
        var organizationId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            null);

        var membership = Membership.Create(
            organizationId,
            Guid.NewGuid(),
            Guid.NewGuid());

        membership.Deactivate();

        var unitOfWork = new FakeUnitOfWork();
        var auditLogger = new FakeAuditLogger();

        var handler = new ChangeTeamLeadHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeTeamRepository
            {
                Team = team
            },
            new FakeMembershipRepository
            {
                Membership = membership
            },
            auditLogger,
            unitOfWork);

        var action = () => handler.Handle(
            new ChangeTeamLeadCommand(team.Id, membership.Id),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Active organization membership was not found.");

        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenMembershipIsNotTeamMember()
    {
        var organizationId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            null);

        var membership = Membership.Create(
            organizationId,
            Guid.NewGuid(),
            Guid.NewGuid());

        var teamRepository = new FakeTeamRepository
        {
            Team = team,
            IsTeamMember = false
        };

        var unitOfWork = new FakeUnitOfWork();
        var auditLogger = new FakeAuditLogger();

        var handler = new ChangeTeamLeadHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            teamRepository,
            new FakeMembershipRepository
            {
                Membership = membership
            },
            auditLogger,
            unitOfWork);

        var action = () => handler.Handle(
            new ChangeTeamLeadCommand(team.Id, membership.Id),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("The team lead must be a member of the team.");

        team.TeamLeadMembershipId.Should().BeNull();
        teamRepository.IsTeamMemberCallCount.Should().Be(1);
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private sealed class FakeTenantContext(Guid organizationId)
        : ITenantContext
    {
        public Task<Guid> GetOrganizationIdAsync(
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(organizationId);
    }

    private sealed class FakeCurrentUser(Guid userId)
        : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeTeamRepository : ITeamRepository
    {
        public Team? Team { get; init; }
        public bool IsTeamMember { get; init; }
        public int IsTeamMemberCallCount { get; private set; }

        public Task AddAsync(
            Team team,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.CompletedTask;

        public Task<Team?> GetByIdAsync(
            Guid organizationId,
            Guid teamId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(Team);

        public Task<bool> ExistsByNormalizedNameAsync(
            Guid organizationId,
            string normalizedName,
            Guid? excludingTeamId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(false);

        public Task<IReadOnlyList<TeamRecord>> GetOrganizationTeamsAsync(
            Guid organizationId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult<IReadOnlyList<TeamRecord>>([]);

        public Task<IReadOnlyList<TeamMemberRecord>> GetTeamMembersAsync(
            Guid organizationId,
            Guid teamId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult<IReadOnlyList<TeamMemberRecord>>([]);

        public Task<bool> IsTeamMemberAsync(
            Guid organizationId,
            Guid teamId,
            Guid membershipId,
            CancellationToken cancellationToken)
        {
            IsTeamMemberCallCount++;
            return System.Threading.Tasks.Task.FromResult(IsTeamMember);
        }

        public Task AddMemberAsync(
            TeamMember teamMember,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.CompletedTask;

        public Task<TeamMember?> GetTeamMemberAsync(
            Guid organizationId,
            Guid teamId,
            Guid membershipId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult<TeamMember?>(null);

        public void RemoveMember(TeamMember teamMember)
        {
        }
    }

    private sealed class FakeMembershipRepository
        : IMembershipRepository
    {
        public Membership? Membership { get; init; }

        public Task AddAsync(
            Membership membership,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.CompletedTask;

        public Task<Membership?> GetByIdAsync(
            Guid organizationId,
            Guid membershipId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(Membership);

        public Task<IReadOnlyList<OrganizationMemberRecord>> GetOrganizationMembersAsync(
            Guid organizationId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult<IReadOnlyList<OrganizationMemberRecord>>([]);

        public Task<bool> IsActiveMemberAsync(
            Guid organizationId,
            Guid userId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(Membership?.IsActive ?? false);
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

            return System.Threading.Tasks.Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;
            return System.Threading.Tasks.Task.FromResult(1);
        }
    }
}
