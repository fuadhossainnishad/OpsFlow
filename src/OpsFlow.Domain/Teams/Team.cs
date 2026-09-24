using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Teams;

public sealed class Team : Entity
{
    private Team()
    {
    }

    private Team(
        Guid id,
        Guid organizationId,
        string name,
        string normalizedName,
        string? description)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        NormalizedName = normalizedName;
        Description = description;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid OrganizationId { get; private init; }

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    public string? Description { get; private set; }

    public Guid? TeamLeadMembershipId { get; private set; }

    public bool IsArchived { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public static Team Create(
        Guid organizationId,
        string name,
        string? description)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization ID is required.", nameof(organizationId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmedName = name.Trim();

        if (trimmedName.Length > 150)
        {
            throw new ArgumentException("Team name cannot exceed 150 characters.", nameof(name));
        }

        return new Team(
            Guid.NewGuid(),
            organizationId,
            trimmedName,
            trimmedName.ToUpperInvariant(),
            NormalizeDescription(description));
    }

    public void Update(string name, string? description)
    {
        if (IsArchived)
        {
            throw new InvalidOperationException("Archived teams cannot be updated.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmedName = name.Trim();

        if (trimmedName.Length > 150)
        {
            throw new ArgumentException("Team name cannot exceed 150 characters.", nameof(name));
        }

        Name = trimmedName;
        NormalizedName = trimmedName.ToUpperInvariant();
        Description = NormalizeDescription(description);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void SetTeamLead(Guid membershipId)
    {
        if (IsArchived)
        {
            throw new InvalidOperationException("Archived teams cannot have their lead changed.");
        }

        if (membershipId == Guid.Empty)
        {
            throw new ArgumentException("Membership ID is required.", nameof(membershipId));
        }

        TeamLeadMembershipId = membershipId;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        if (IsArchived)
        {
            return;
        }

        IsArchived = true;
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var trimmed = description.Trim();

        return trimmed.Length == 0 ? null : trimmed;
    }
}
