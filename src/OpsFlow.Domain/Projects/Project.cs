using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Projects;

public sealed class Project : Entity
{
    private Project()
    {
    }

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
        CreatedAtUtc = DateTimeOffset.UtcNow;
        Status = ProjectStatus.Active;
    }

    public Guid OrganizationId { get; private init; }

    public string Name { get; private set; } = null!;

    public string Key { get; private set; } = null!;

    public string? Description { get; private set; }

    public ProjectStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public DateTimeOffset? ArchivedAtUtc { get; private set; }

    public static Project Create(
        Guid organizationId,
        string name,
        string key,
        string? description)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID is required.",
                nameof(organizationId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var trimmedName = name.Trim();
        var normalizedKey = key.Trim().ToUpperInvariant();

        if (trimmedName.Length > 200)
        {
            throw new ArgumentException(
                "Project name cannot exceed 200 characters.",
                nameof(name));
        }

        if (normalizedKey.Length > 50)
        {
            throw new ArgumentException(
                "Project key cannot exceed 50 characters.",
                nameof(key));
        }

        return new Project(
            Guid.NewGuid(),
            organizationId,
            trimmedName,
            normalizedKey,
            string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim());
    }

    public void Update(
        string name,
        string? description)
    {
        if (Status == ProjectStatus.Archived)
        {
            throw new InvalidOperationException(
                "Archived projects cannot be updated.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmedName = name.Trim();

        if (trimmedName.Length > 200)
        {
            throw new ArgumentException(
                "Project name cannot exceed 200 characters.",
                nameof(name));
        }

        Name = trimmedName;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }

    public void Archive()
    {
        if (Status == ProjectStatus.Archived)
        {
            return;
        }

        Status = ProjectStatus.Archived;
        ArchivedAtUtc = DateTimeOffset.UtcNow;
    }
}
