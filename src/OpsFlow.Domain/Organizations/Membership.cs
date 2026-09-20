using OpsFlow.Domain.Common;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Domain.Organizations;

public sealed class Membership : Entity
{
    private Membership()
    {
    }

    private Membership(
        Guid id,
        Guid organizationId,
        Guid userId,
        Guid roleId)
    {
        Id = id;
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }

    public Guid OrganizationId { get; private init; }

    public Guid UserId { get; private init; }

    public Guid RoleId { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset JoinedAtUtc { get; private init; }

    public static Membership Create(
        Guid organizationId,
        Guid userId,
        Guid roleId)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID cannot be empty.",
                nameof(organizationId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));
        }

        return new Membership(
            Guid.NewGuid(),
            organizationId,
            userId,
            roleId)
        {
            JoinedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void ChangeRole(Guid roleId)
    {
        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));
        }

        RoleId = roleId;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
