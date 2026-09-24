using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Security;
using OpsFlow.Domain.Identity;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Identity.RegisterUser;

public sealed class RegisterUserHandler(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
{
    public async Task<RegisterUserResult> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = command.Email.Trim();
        var firstName = command.FirstName.Trim();
        var lastName = command.LastName.Trim();

        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Password);

        var normalizedEmail = email.ToUpperInvariant();

        var existingUser = await userRepository
            .GetByNormalizedEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "A user with this email already exists.");
        }

        var user = User.Create(
            email,
            firstName,
            lastName);

        var passwordHash = passwordHasher.Hash(command.Password);

        var credential = UserCredential.Create(
            user.Id,
            passwordHash);

        await userRepository.AddAsync(
            user,
            cancellationToken);

        await userCredentialRepository.AddAsync(
            credential,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName);
    }
}
