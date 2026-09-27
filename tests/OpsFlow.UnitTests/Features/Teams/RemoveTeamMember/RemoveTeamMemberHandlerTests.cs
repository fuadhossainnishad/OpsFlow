using FluentAssertions;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Teams;
using OpsFlow.Application.Features.Teams.RemoveTeamMember;
using OpsFlow.Domain.Teams;

namespace OpsFlow.UnitTests.Features.Teams.RemoveTeamMember;

public sealed class RemoveTeamMemberHandlerTests
{
    [Fact]
    public async Task HandleShouldRemoveMemberAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            "Engineering team");

        var member = TeamMember.Create(
            teamId,
            membershipId);

        var tenantContext = new FakeTenantContext(organizationId);
        var currentUser = new FakeCurrentUser(userId);
        var teamRepository = new FakeTeamRepository
        {
            Team = team,
            Member = member
        };
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new RemoveTeamMemberHandler(
            tenantContext,
            currentUser,
            teamRepository,
            auditLogger,
            unitOfWork);

        var result = await handler.Handle(
            new RemoveTeamMemberCommand(teamId, membershipId),
            CancellationToken.None);

        result.TeamId.Should().Be(teamId);
        result.MembershipId.Should().Be(membershipId);

        teamRepository.RemovedMember.Should().BeSameAs(member);
        auditLogger.CallCount.Should().Be(1);
        auditLogger.OrganizationId.Should().Be(organizationId);
        auditLogger.ActorUserId.Should().Be(userId);
        auditLogger.Action.Should().Be("team.member_removed");
        auditLogger.Resource.Should().Be("team");
        auditLogger.ResourceId.Should().Be(team.Id);
        auditLogger.BeforeJson.Should().NotBeNull();
        auditLogger.BeforeJson.Should().Contain(member.Id.ToString());
        auditLogger.BeforeJson.Should().Contain(membershipId.ToString());
        auditLogger.AfterJson.Should().BeNull();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectWhenTeamDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var teamRepository = new FakeTeamRepository();

        var handler = CreateHandler(
            organizationId,
            Guid.NewGuid(),
            teamRepository);

        var action = () => handler.Handle(
            new RemoveTeamMemberCommand(
                Guid.NewGuid(),
                Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Team was not found.");

        teamRepository.RemovedMember.Should().BeNull();
    }

    [Fact]
    public async Task HandleShouldRejectWhenMemberDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            null);

        var teamRepository = new FakeTeamRepository
        {
            Team = team
        };

        var handler = CreateHandler(
            organizationId,
            Guid.NewGuid(),
            teamRepository);

        var action = () => handler.Handle(
            new RemoveTeamMemberCommand(
                teamId,
                Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Team member was not found.");

        teamRepository.RemovedMember.Should().BeNull();
    }

    [Fact]
    public async Task HandleShouldRejectRemovingCurrentTeamLead()
    {
        var organizationId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "Engineering",
            null);

        team.SetTeamLead(membershipId);

        var member = TeamMember.Create(
            teamId,
            membershipId);

        var teamRepository = new FakeTeamRepository
        {
            Team = team,
            Member = member
        };

        var unitOfWork = new FakeUnitOfWork();
        var auditLogger = new FakeAuditLogger();

        var handler = new RemoveTeamMemberHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            teamRepository,
            auditLogger,
            unitOfWork);

        var action = () => handler.Handle(
            new RemoveTeamMemberCommand(teamId, membershipId),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("The current team lead cannot be removed until another lead is assigned.");

        teamRepository.RemovedMember.Should().BeNull();
        auditLogger.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static RemoveTeamMemberHandler CreateHandler(
        Guid organizationId,
        Guid currentUserId,
        FakeTeamRepository teamRepository)
    {
        return new RemoveTeamMemberHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(currentUserId),
            teamRepository,
            new FakeAuditLogger(),
            new FakeUnitOfWork());
    }

    private sealed class FakeTenantContext(Guid organizationId)
        : ITenantContext
    {
        public Task<Guid> GetOrganizationIdAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(organizationId);
        }
    }

    private sealed class FakeCurrentUser(Guid userId)
        : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeTeamRepository : ITeamRepository
    {
        public Team? Team { get; init; }

        public TeamMember? Member { get; init; }

        public TeamMember? RemovedMember { get; private set; }

        public Task AddAsync(
            Team team,
            CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<Team?> GetByIdAsync(
            Guid organizationId,
            Guid teamId,
            CancellationToken cancellationToken)
            => Task.FromResult(Team);

        public Task<bool> ExistsByNormalizedNameAsync(
            Guid organizationId,
            string normalizedName,
            Guid? excludingTeamId,
            CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<IReadOnlyList<TeamRecord>> GetOrganizationTeamsAsync(
            Guid organizationId,
            CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<TeamRecord>>([]);

        public Task<IReadOnlyList<TeamMemberRecord>> GetTeamMembersAsync(
            Guid organizationId,
            Guid teamId,
            CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<TeamMemberRecord>>([]);

        public Task<bool> IsTeamMemberAsync(
            Guid organizationId,
            Guid teamId,
            Guid membershipId,
            CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task AddMemberAsync(
            TeamMember teamMember,
            CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<TeamMember?> GetTeamMemberAsync(
            Guid organizationId,
            Guid teamId,
            Guid membershipId,
            CancellationToken cancellationToken)
            => Task.FromResult(Member);

        public void RemoveMember(TeamMember teamMember)
        {
            RemovedMember = teamMember;
        }
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
