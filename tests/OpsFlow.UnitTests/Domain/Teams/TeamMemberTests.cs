using FluentAssertions;
using OpsFlow.Domain.Teams;

namespace OpsFlow.UnitTests.Domain.Teams;

public sealed class TeamMemberTests
{
    [Fact]
    public void CreateShouldInitializeMembership()
    {
        var teamId = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var member = TeamMember.Create(teamId, membershipId);

        member.TeamId.Should().Be(teamId);
        member.MembershipId.Should().Be(membershipId);
        member.Id.Should().NotBe(Guid.Empty);
        member.AddedAtUtc.Should().BeCloseTo(
            DateTimeOffset.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CreateShouldRejectEmptyTeamId()
    {
        var action = () => TeamMember.Create(
            Guid.Empty,
            Guid.NewGuid());

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectEmptyMembershipId()
    {
        var action = () => TeamMember.Create(
            Guid.NewGuid(),
            Guid.Empty);

        action.Should().Throw<ArgumentException>();
    }
}
