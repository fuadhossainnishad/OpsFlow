using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Identity;

public sealed class RefreshToken : Entity
{
    private RefreshToken()
    {
    }

    private RefreshToken(
        Guid id,
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc)
    {
        Id = id;
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    public Guid UserId { get; private init; }

    public string TokenHash { get; private set; } = null!;

    public DateTimeOffset ExpiresAtUtc { get; private init; }

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public DateTimeOffset? RevokedAtUtc { get; private set; }

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsExpired =>
        DateTimeOffset.UtcNow >= ExpiresAtUtc;

    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTimeOffset expiresAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException(
                "Refresh token expiration must be in the future.",
                nameof(expiresAtUtc));
        }

        return new RefreshToken(
            Guid.NewGuid(),
            userId,
            tokenHash,
            expiresAtUtc)
        {
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void Revoke()
    {
        if (!IsRevoked)
        {
            RevokedAtUtc = DateTimeOffset.UtcNow;
        }
    }
}
