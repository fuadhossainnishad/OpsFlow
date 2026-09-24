namespace OpsFlow.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = null!;

    public string Audience { get; init; } = null!;

    public string SigningKey { get; init; } = null!;

    public int AccessTokenLifetimeMinutes { get; init; } = 15;
}
