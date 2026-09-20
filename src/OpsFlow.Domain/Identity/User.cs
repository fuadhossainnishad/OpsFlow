using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Identity;

public sealed class User : Entity
{
    private User()
    {
    }

    private User(
        Guid id,
        string email,
        string normalizedEmail,
        string firstName,
        string lastName)
    {
        Id = id;
        Email = email;
        NormalizedEmail = normalizedEmail;
        FirstName = firstName;
        LastName = lastName;
    }

    public string Email { get; private set; } = null!;

    public string NormalizedEmail { get; private set; } = null!;

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public static User Create(
        string email,
        string firstName,
        string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        var normalizedEmail = email.Trim().ToUpperInvariant();

        return new User(
            Guid.NewGuid(),
            email.Trim(),
            normalizedEmail,
            firstName.Trim(),
            lastName.Trim())
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

