#pragma warning disable CA1711

namespace OpsFlow.Domain.Authorization;

public sealed class RolePermission
{
    private RolePermission()
    {
    }

    private RolePermission(
        Guid roleId,
        Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    public Guid RoleId { get; private init; }

    public Guid PermissionId { get; private init; }

    public static RolePermission Create(
        Guid roleId,
        Guid permissionId)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));
        }

        if (permissionId == Guid.Empty)
        {
            throw new ArgumentException(
                "Permission ID cannot be empty.",
                nameof(permissionId));
        }

        return new RolePermission(roleId, permissionId);
    }
}

#pragma warning disable CA1711
