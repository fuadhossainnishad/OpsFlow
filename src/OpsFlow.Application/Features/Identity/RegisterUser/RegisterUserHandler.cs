using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Security;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Application.Features.Identity.RegisterUser;

public sealed class RegisterUserHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher)
{
    public async Task<RegisterUserResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = command.Email.Trim();

        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.LastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Password);

        var normalizedEmail = email.ToUpperInvariant();

        var existingUser = await userRepository
            .GetByNormalizedEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var passwordHash = passwordHasher.Hash(command.Password);

        var user = User.Create(
            email,
            command.FirstName,
            command.LastName);

        // Password persistence will be introduced into the
        // authentication model before completing this slice.

        await userRepository.AddAsync(
            user,
            cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName);
    }
}
