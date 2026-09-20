using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Organizations;

public sealed class Organization : Entity
{
    private Organization()
    {
    }

    private Organization(
        Guid id,
        string name,
        string slug)
    {
        Id = id;
        Name = name;
        Slug = slug;
    }

    public string Name { get; private set; } = null!;

    public string Slug { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public static Organization Create(
        string name,
        string slug)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        return new Organization(
            Guid.NewGuid(),
            name.Trim(),
            slug.Trim().ToLowerInvariant())
        {
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
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
