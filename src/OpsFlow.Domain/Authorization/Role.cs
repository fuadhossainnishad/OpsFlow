using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Authorization;

public sealed class Role : Entity
{
    private Role()
    {
    }

    private Role(
        Guid id,
        string name,
        string normalizedName,
        bool isSystemRole)
    {
        Id = id;
        Name = name;
        NormalizedName = normalizedName;
        IsSystemRole = isSystemRole;
    }

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    public bool IsSystemRole { get; private init; }

    public static Role Create(
        string name,
        bool isSystemRole = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmedName = name.Trim();

        return new Role(
            Guid.NewGuid(),
            trimmedName,
            trimmedName.ToUpperInvariant(),
            isSystemRole);
    }
}
