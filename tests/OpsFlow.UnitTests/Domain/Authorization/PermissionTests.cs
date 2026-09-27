using FluentAssertions;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.UnitTests.Domain.Authorization;

public sealed class PermissionTests
{
    [Fact]
    public void CreateShouldNormalizeCodeAndTrimName()
    {
        var permission = Permission.Create(
            "  ORGANIZATIONS.CREATE  ",
            "  Create Organizations  ");

        permission.Id.Should().NotBeEmpty();
        permission.Code.Should().Be("organizations.create");
        permission.Name.Should().Be("Create Organizations");
    }

    [Fact]
    public void CreateShouldRejectEmptyCode()
    {
        var action = () => Permission.Create(
            "   ",
            "Create Organizations");

        action.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectEmptyName()
    {
        var action = () => Permission.Create(
            "organizations.create",
            "   ");

        action.Should()
            .Throw<ArgumentException>();
    }
}
