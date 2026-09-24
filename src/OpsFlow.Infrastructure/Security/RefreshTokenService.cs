using System.Security.Cryptography;
using System.Text;
using OpsFlow.Application.Abstractions.Security;

namespace OpsFlow.Infrastructure.Security;

public sealed class RefreshTokenService : IRefreshTokenService
{
    public string GenerateToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }

    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}
