using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Security;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Domain.Identity;

namespace OpsFlow.Application.Features.Identity.Login;

public sealed class LoginUserHandler(
    IUserRepository userRepository,
    IUserCredentialRepository userCredentialRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<LoginUserResult> HandleAsync(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var email = command.Email.Trim();

        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Password);

        var normalizedEmail = email.ToUpperInvariant();

        var user = await userRepository
            .GetByNormalizedEmailAsync(
                normalizedEmail,
                cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var credential = await userCredentialRepository
            .GetByUserIdAsync(
                user.Id,
                cancellationToken);

        if (credential is null ||
            !passwordHasher.Verify(
                command.Password,
                credential.PasswordHash))
        {
            throw new UnauthorizedException(
                "Invalid email or password.");
        }

        var accessToken = accessTokenService.CreateToken(
            user.Id);

        var refreshToken = refreshTokenService.GenerateToken();

        var refreshTokenHash = refreshTokenService.HashToken(
            refreshToken);

        var tokenEntity = RefreshToken.Create(
            user.Id,
            refreshTokenHash,
            DateTimeOffset.UtcNow.AddDays(30));

        await refreshTokenRepository.AddAsync(
      tokenEntity,
      cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new LoginUserResult(
            user.Id,
            accessToken,
            refreshToken);
    }
}
