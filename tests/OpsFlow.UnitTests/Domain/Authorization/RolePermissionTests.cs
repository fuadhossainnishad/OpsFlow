using FluentAssertions;
using OpsFlow.Domain.Authorization;

namespace OpsFlow.UnitTests.Domain.Authorization;

public sealed class RolePermissionTests
{
    [Fact]
    public void CreateShouldCreateRolePermission()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var rolePermission = RolePermission.Create(
            roleId,
            permissionId);

        rolePermission.RoleId.Should().Be(roleId);
        rolePermission.PermissionId.Should().Be(permissionId);
    }

    [Fact]
    public void CreateShouldRejectEmptyRoleId()
    {
        var action = () => RolePermission.Create(
            Guid.Empty,
            Guid.NewGuid());

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Role ID cannot be empty.*");
    }

    [Fact]
    public void CreateShouldRejectEmptyPermissionId()
    {
        var action = () => RolePermission.Create(
            Guid.NewGuid(),
            Guid.Empty);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Permission ID cannot be empty.*");
    }
}
