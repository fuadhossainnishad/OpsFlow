using FluentAssertions;
using OpsFlow.Domain.Teams;

namespace OpsFlow.UnitTests.Domain.Teams;

public sealed class TeamTests
{
    [Fact]
    public void CreateShouldNormalizeValues()
    {
        var team = Team.Create(
            Guid.NewGuid(),
            "  Engineering  ",
            "  Backend team  ");

        team.Name.Should().Be("Engineering");
        team.NormalizedName.Should().Be("ENGINEERING");
        team.Description.Should().Be("Backend team");
        team.IsArchived.Should().BeFalse();
        team.TeamLeadMembershipId.Should().BeNull();
    }

    [Fact]
    public void CreateShouldConvertBlankDescriptionToNull()
    {
        var team = Team.Create(
            Guid.NewGuid(),
            "Engineering",
            "   ");

        team.Description.Should().BeNull();
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => Team.Create(
            Guid.Empty,
            "Engineering",
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectBlankName()
    {
        var action = () => Team.Create(
            Guid.NewGuid(),
            "   ",
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectNameOver150Characters()
    {
        var action = () => Team.Create(
            Guid.NewGuid(),
            new string('x', 151),
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateShouldNormalizeValues()
    {
        var team = CreateTeam();

        team.Update(
            "  Product  ",
            "  Product team  ");

        team.Name.Should().Be("Product");
        team.NormalizedName.Should().Be("PRODUCT");
        team.Description.Should().Be("Product team");
        team.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void UpdateShouldConvertBlankDescriptionToNull()
    {
        var team = CreateTeam();

        team.Update("Product", "   ");

        team.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateShouldRejectNameOver150Characters()
    {
        var team = CreateTeam();

        var action = () => team.Update(
            new string('x', 151),
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateShouldRejectArchivedTeam()
    {
        var team = CreateTeam();
        team.Archive();

        var action = () => team.Update("Updated", null);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SetTeamLeadShouldAssignMembership()
    {
        var team = CreateTeam();
        var membershipId = Guid.NewGuid();

        team.SetTeamLead(membershipId);

        team.TeamLeadMembershipId.Should().Be(membershipId);
        team.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void SetTeamLeadShouldRejectEmptyMembershipId()
    {
        var team = CreateTeam();

        var action = () => team.SetTeamLead(Guid.Empty);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetTeamLeadShouldRejectArchivedTeam()
    {
        var team = CreateTeam();
        team.Archive();

        var action = () => team.SetTeamLead(Guid.NewGuid());

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ArchiveShouldArchiveTeam()
    {
        var team = CreateTeam();

        team.Archive();

        team.IsArchived.Should().BeTrue();
        team.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void ArchiveShouldBeIdempotent()
    {
        var team = CreateTeam();

        team.Archive();
        var firstUpdatedAt = team.UpdatedAtUtc;

        team.Archive();

        team.IsArchived.Should().BeTrue();
        team.UpdatedAtUtc.Should().Be(firstUpdatedAt);
    }

    private static Team CreateTeam() =>
        Team.Create(
            Guid.NewGuid(),
            "Engineering",
            null);
}
