#pragma warning disable CA1711

using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Authorization;

public sealed class Permission : Entity
{
    private Permission()
    {
    }

    private Permission(
        Guid id,
        string code,
        string name)
    {
        Id = id;
        Code = code;
        Name = name;
    }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public static Permission Create(
        string code,
        string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Permission(
            Guid.NewGuid(),
            code.Trim().ToLowerInvariant(),
            name.Trim());
    }
}

#pragma warning disable CA1711
