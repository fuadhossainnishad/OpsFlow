using System.Security.Cryptography;
using System.Text;

namespace OpsFlow.Application.Features.Members.Common;

internal static class InvitationToken
{
    public static string Generate()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var hash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(hash);
    }
}
