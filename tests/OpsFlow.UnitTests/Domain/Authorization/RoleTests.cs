using FluentAssertions;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.UnitTests.Domain.Authorization;

public sealed class RoleTests
{
    [Fact]
    public void CreateShouldCreateRoleWithNormalizedName()
    {
        var role = Role.Create("  Project Manager  ");

        role.Id.Should().NotBeEmpty();
        role.Name.Should().Be("Project Manager");
        role.NormalizedName.Should().Be("PROJECT MANAGER");
        role.IsSystemRole.Should().BeFalse();
    }

    [Fact]
    public void CreateShouldSetSystemRoleFlag()
    {
        var role = Role.Create("Administrator", isSystemRole: true);

        role.IsSystemRole.Should().BeTrue();
        role.Name.Should().Be("Administrator");
        role.NormalizedName.Should().Be("ADMINISTRATOR");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectInvalidName(string name)
    {
        var action = () => Role.Create(name);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectNullName()
    {
        var action = () => Role.Create(null!);

        action.Should().Throw<ArgumentNullException>();
    }
}
