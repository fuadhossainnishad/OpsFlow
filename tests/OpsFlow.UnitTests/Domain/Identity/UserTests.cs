using FluentAssertions;
using OpsFlow.Domain.Identity;

namespace OpsFlow.UnitTests.Domain.Identity;

public sealed class UserTests
{
    [Fact]
    public void CreateShouldCreateActiveUserWithNormalizedEmail()
    {
        var user = User.Create(
            "  fuad@example.com  ",
            "Fuad",
            "Hossain");

        user.Email.Should().Be("fuad@example.com");
        user.NormalizedEmail.Should().Be("FUAD@EXAMPLE.COM");
        user.FirstName.Should().Be("Fuad");
        user.LastName.Should().Be("Hossain");
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
        user.CreatedAtUtc.Should().NotBe(default);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void CreateShouldRejectEmptyEmail(string email)
    {
        var action = () => User.Create(
            email,
            "Fuad",
            "Hossain");

        action.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void DeactivateShouldMakeUserInactive()
    {
        var user = User.Create(
            "fuad@example.com",
            "Fuad",
            "Hossain");

        user.Deactivate();

        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ActivateShouldMakeUserActive()
    {
        var user = User.Create(
            "fuad@example.com",
            "Fuad",
            "Hossain");

        user.Deactivate();
        user.Activate();

        user.IsActive.Should().BeTrue();
    }
}
