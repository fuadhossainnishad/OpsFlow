using FluentAssertions;
using OpsFlow.Domain.Identity;

namespace OpsFlow.UnitTests.Domain.Identity;

public sealed class UserCredentialTests
{
    [Fact]
    public void CreateShouldInitializeCredential()
    {
        var userId = Guid.NewGuid();

        var credential = UserCredential.Create(
            userId,
            "hashed-password");

        credential.UserId.Should().Be(userId);
        credential.PasswordHash.Should().Be("hashed-password");
        credential.Id.Should().NotBe(Guid.Empty);
        credential.PasswordChangedAtUtc.Should().BeCloseTo(
            DateTimeOffset.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CreateShouldRejectEmptyUserId()
    {
        var action = () => UserCredential.Create(
            Guid.Empty,
            "hashed-password");

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*User ID cannot be empty*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankPasswordHash(string passwordHash)
    {
        var action = () => UserCredential.Create(
            Guid.NewGuid(),
            passwordHash);

        action.Should()
            .Throw<ArgumentException>()
            .WithParameterName(nameof(passwordHash));
    }

    [Fact]
    public void ChangePasswordShouldUpdateHashAndTimestamp()
    {
        var credential = UserCredential.Create(
            Guid.NewGuid(),
            "old-hash");

        var previousChangedAt = credential.PasswordChangedAtUtc;

        credential.ChangePassword("new-hash");

        credential.PasswordHash.Should().Be("new-hash");
        credential.PasswordChangedAtUtc.Should().BeOnOrAfter(previousChangedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ChangePasswordShouldRejectBlankPasswordHash(string passwordHash)
    {
        var credential = UserCredential.Create(
            Guid.NewGuid(),
            "old-hash");

        var action = () => credential.ChangePassword(passwordHash);

        action.Should()
            .Throw<ArgumentException>()
            .WithParameterName(nameof(passwordHash));
    }
}
