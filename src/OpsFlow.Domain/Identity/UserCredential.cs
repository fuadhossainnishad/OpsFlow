using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Identity;

public sealed class UserCredential : Entity
{
    private UserCredential()
    {
    }

    private UserCredential(
        Guid id,
        Guid userId,
        string passwordHash)
    {
        Id = id;
        UserId = userId;
        PasswordHash = passwordHash;
    }

    public Guid UserId { get; private init; }

    public string PasswordHash { get; private set; } = null!;

    public DateTimeOffset PasswordChangedAtUtc { get; private set; }

    public static UserCredential Create(
        Guid userId,
        string passwordHash)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        return new UserCredential(
            Guid.NewGuid(),
            userId,
            passwordHash)
        {
            PasswordChangedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void ChangePassword(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        PasswordHash = passwordHash;
        PasswordChangedAtUtc = DateTimeOffset.UtcNow;
    }
}
