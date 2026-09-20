using FluentAssertions;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.UnitTests.Domain.Organizations;

public sealed class MembershipTests
{
    [Fact]
    public void CreateShouldCreateActiveMembership()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var membership = Membership.Create(
            organizationId,
            userId,
            roleId);

        membership.OrganizationId.Should().Be(organizationId);
        membership.UserId.Should().Be(userId);
        membership.RoleId.Should().Be(roleId);
        membership.IsActive.Should().BeTrue();
        membership.Id.Should().NotBeEmpty();
        membership.JoinedAtUtc.Should().NotBe(default);
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => Membership.Create(
            Guid.Empty,
            Guid.NewGuid(),
            Guid.NewGuid());

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Organization ID*");
    }

    [Fact]
    public void CreateShouldRejectEmptyUserId()
    {
        var action = () => Membership.Create(
            Guid.NewGuid(),
            Guid.Empty,
            Guid.NewGuid());

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*User ID*");
    }

    [Fact]
    public void CreateShouldRejectEmptyRoleId()
    {
        var action = () => Membership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Role ID*");
    }

    [Fact]
    public void ChangeRoleShouldUpdateRole()
    {
        var membership = Membership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        var newRoleId = Guid.NewGuid();

        membership.ChangeRole(newRoleId);

        membership.RoleId.Should().Be(newRoleId);
    }

    [Fact]
    public void DeactivateShouldMakeMembershipInactive()
    {
        var membership = Membership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        membership.Deactivate();

        membership.IsActive.Should().BeFalse();
    }
}
