namespace OpsFlow.Application.Abstractions.Security;

public interface IRefreshTokenService
{
    string GenerateToken();

    string HashToken(string token);
}
