using FluentAssertions;
using OpsFlow.Domain.Identity;

namespace OpsFlow.UnitTests.Domain.Identity;

public sealed class RefreshTokenTests
{
    [Fact]
    public void CreateShouldInitializeToken()
    {
        var userId = Guid.NewGuid();
        var expiresAt = DateTimeOffset.UtcNow.AddDays(7);

        var token = RefreshToken.Create(
            userId,
            "token-hash",
            expiresAt);

        token.Id.Should().NotBeEmpty();
        token.UserId.Should().Be(userId);
        token.TokenHash.Should().Be("token-hash");
        token.ExpiresAtUtc.Should().Be(expiresAt);
        token.CreatedAtUtc.Should().BeCloseTo(
            DateTimeOffset.UtcNow,
            TimeSpan.FromSeconds(2));
        token.IsRevoked.Should().BeFalse();
        token.IsExpired.Should().BeFalse();
    }

    [Fact]
    public void CreateShouldRejectEmptyUserId()
    {
        var action = () => RefreshToken.Create(
            Guid.Empty,
            "hash",
            DateTimeOffset.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectExpiredToken()
    {
        var action = () => RefreshToken.Create(
            Guid.NewGuid(),
            "hash",
            DateTimeOffset.UtcNow.AddMinutes(-1));

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*expiration must be in the future*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankTokenHash(string tokenHash)
    {
        var action = () => RefreshToken.Create(
            Guid.NewGuid(),
            tokenHash,
            DateTimeOffset.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RevokeShouldMarkTokenAsRevoked()
    {
        var token = CreateToken();

        token.Revoke();

        token.IsRevoked.Should().BeTrue();
        token.RevokedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void RevokeShouldBeIdempotent()
    {
        var token = CreateToken();

        token.Revoke();
        var firstRevokedAt = token.RevokedAtUtc;

        token.Revoke();

        token.RevokedAtUtc.Should().Be(firstRevokedAt);
    }

    [Fact]
    public void IsExpiredShouldBeTrueForExpiredToken()
    {
        var token = CreateToken();

        var property = typeof(RefreshToken)
            .GetProperty(
                nameof(RefreshToken.ExpiresAtUtc),
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

        property!.SetValue(
            token,
            DateTimeOffset.UtcNow.AddMinutes(-1));

        token.IsExpired.Should().BeTrue();
    }

    private static RefreshToken CreateToken()
    {
        return RefreshToken.Create(
            Guid.NewGuid(),
            "token-hash",
            DateTimeOffset.UtcNow.AddDays(7));
    }
}
