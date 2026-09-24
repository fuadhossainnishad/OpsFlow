using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Projects;

public sealed class Project : Entity
{
    private Project() { }

    private Project(
        Guid id,
        Guid organizationId,
        string name,
        string key,
        string? description)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        Key = key;
        Description = description;
    }

    public Guid OrganizationId { get; private init; }

    public string Name { get; private set; } = null!;

    public string Key { get; private set; } = null!;

    public string? Description { get; private set; }

    public ProjectStatus Status { get; private set; } =
        ProjectStatus.Active;

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public static Project Create(
        Guid organizationId,
        string name,
        string key,
        string? description)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID cannot be empty.",
                nameof(organizationId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        return new Project(
            Guid.NewGuid(),
            organizationId,
            name.Trim(),
            key.Trim().ToUpperInvariant(),
            string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim())
        {
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void Archive()
    {
        Status = ProjectStatus.Archived;
    }

    public void Activate()
    {
        Status = ProjectStatus.Active;
    }
}
