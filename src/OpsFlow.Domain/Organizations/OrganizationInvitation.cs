using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Organizations;

public sealed class OrganizationInvitation : Entity
{
    private OrganizationInvitation()
    {
    }

    private OrganizationInvitation(
        Guid id,
        Guid organizationId,
        string email,
        string normalizedEmail,
        Guid roleId,
        string tokenHash,
        DateTimeOffset expiresAtUtc)
    {
        Id = id;
        OrganizationId = organizationId;
        Email = email;
        NormalizedEmail = normalizedEmail;
        RoleId = roleId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid OrganizationId { get; private init; }

    public string Email { get; private init; } = null!;

    public string NormalizedEmail { get; private init; } = null!;

    public Guid RoleId { get; private init; }

    public string TokenHash { get; private init; } = null!;

    public DateTimeOffset ExpiresAtUtc { get; private init; }

    public DateTimeOffset? AcceptedAtUtc { get; private set; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public bool IsPending =>
        AcceptedAtUtc is null &&
        RevokedAtUtc is null &&
        ExpiresAtUtc > DateTimeOffset.UtcNow;

    public static OrganizationInvitation Create(
        Guid organizationId,
        string email,
        Guid roleId,
        string tokenHash,
        DateTimeOffset expiresAtUtc)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID cannot be empty.",
                nameof(organizationId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (roleId == Guid.Empty)
        {
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException(
                "Invitation expiration must be in the future.",
                nameof(expiresAtUtc));
        }

        var trimmedEmail = email.Trim();

        return new OrganizationInvitation(
            Guid.NewGuid(),
            organizationId,
            trimmedEmail,
            trimmedEmail.ToUpperInvariant(),
            roleId,
            tokenHash,
            expiresAtUtc);
    }

    public void Accept()
    {
        if (AcceptedAtUtc.HasValue)
        {
            throw new InvalidOperationException(
                "The invitation has already been accepted.");
        }

        if (RevokedAtUtc.HasValue)
        {
            throw new InvalidOperationException(
                "The invitation has been revoked.");
        }

        if (ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException(
                "The invitation has expired.");
        }

        AcceptedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Revoke()
    {
        if (AcceptedAtUtc.HasValue)
        {
            throw new InvalidOperationException(
                "An accepted invitation cannot be revoked.");
        }

        RevokedAtUtc = DateTimeOffset.UtcNow;
    }
}
