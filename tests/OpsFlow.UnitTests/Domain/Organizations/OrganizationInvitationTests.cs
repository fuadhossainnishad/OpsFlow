using FluentAssertions;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.UnitTests.Domain.Organizations;

public sealed class OrganizationInvitationTests
{
    [Fact]
    public void CreateShouldNormalizeAndTrimEmail()
    {
        var organizationId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var invitation = OrganizationInvitation.Create(
            organizationId,
            "  User@Example.com  ",
            roleId,
            "token-hash",
            DateTimeOffset.UtcNow.AddDays(1));

        invitation.OrganizationId.Should().Be(organizationId);
        invitation.Email.Should().Be("User@Example.com");
        invitation.NormalizedEmail.Should().Be("USER@EXAMPLE.COM");
        invitation.RoleId.Should().Be(roleId);
        invitation.TokenHash.Should().Be("token-hash");
        invitation.IsPending.Should().BeTrue();
        invitation.AcceptedAtUtc.Should().BeNull();
        invitation.RevokedAtUtc.Should().BeNull();
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => OrganizationInvitation.Create(
            Guid.Empty,
            "user@example.com",
            Guid.NewGuid(),
            "hash",
            DateTimeOffset.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectEmptyRoleId()
    {
        var action = () => OrganizationInvitation.Create(
            Guid.NewGuid(),
            "user@example.com",
            Guid.Empty,
            "hash",
            DateTimeOffset.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectExpiredInvitation()
    {
        var action = () => OrganizationInvitation.Create(
            Guid.NewGuid(),
            "user@example.com",
            Guid.NewGuid(),
            "hash",
            DateTimeOffset.UtcNow.AddMinutes(-1));

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*expiration must be in the future*");
    }

    [Fact]
    public void AcceptShouldMarkInvitationAsAccepted()
    {
        var invitation = CreateInvitation();

        invitation.Accept();

        invitation.AcceptedAtUtc.Should().NotBeNull();
        invitation.RevokedAtUtc.Should().BeNull();
        invitation.IsPending.Should().BeFalse();
    }

    [Fact]
    public void AcceptShouldRejectAlreadyAcceptedInvitation()
    {
        var invitation = CreateInvitation();
        invitation.Accept();

        var action = () => invitation.Accept();

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The invitation has already been accepted.");
    }

    [Fact]
    public void AcceptShouldRejectRevokedInvitation()
    {
        var invitation = CreateInvitation();
        invitation.Revoke();

        var action = () => invitation.Accept();

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("The invitation has been revoked.");
    }

    [Fact]
    public void RevokeShouldMarkInvitationAsRevoked()
    {
        var invitation = CreateInvitation();

        invitation.Revoke();

        invitation.RevokedAtUtc.Should().NotBeNull();
        invitation.IsPending.Should().BeFalse();
    }

    [Fact]
    public void RevokeShouldRejectAcceptedInvitation()
    {
        var invitation = CreateInvitation();
        invitation.Accept();

        var action = () => invitation.Revoke();

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("An accepted invitation cannot be revoked.");
    }

    [Fact]
    public void IsPendingShouldBeFalseWhenInvitationIsExpired()
    {
        var invitation = CreateInvitation();

        // Use reflection only to exercise the time-dependent expired state.
        var property = typeof(OrganizationInvitation)
            .GetProperty(
                nameof(OrganizationInvitation.ExpiresAtUtc),
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public);

        property!.SetValue(
            invitation,
            DateTimeOffset.UtcNow.AddMinutes(-1));

        invitation.IsPending.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankEmail(string email)
    {
        var action = () => OrganizationInvitation.Create(
            Guid.NewGuid(),
            email,
            Guid.NewGuid(),
            "hash",
            DateTimeOffset.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankTokenHash(string tokenHash)
    {
        var action = () => OrganizationInvitation.Create(
            Guid.NewGuid(),
            "user@example.com",
            Guid.NewGuid(),
            tokenHash,
            DateTimeOffset.UtcNow.AddDays(1));

        action.Should().Throw<ArgumentException>();
    }

    private static OrganizationInvitation CreateInvitation()
    {
        return OrganizationInvitation.Create(
            Guid.NewGuid(),
            "user@example.com",
            Guid.NewGuid(),
            "hash",
            DateTimeOffset.UtcNow.AddDays(1));
    }
}
