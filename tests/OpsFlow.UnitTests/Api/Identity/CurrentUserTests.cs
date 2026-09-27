using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OpsFlow.Api.Identity;

namespace OpsFlow.UnitTests.Api.Identity;

public sealed class CurrentUserTests
{
    [Fact]
    public void UserIdShouldReturnAuthenticatedUserId()
    {
        var userId = Guid.NewGuid();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(
                        System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                        userId.ToString())
                ]))
        };

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var currentUser = new CurrentUser(accessor);

        currentUser.UserId.Should().Be(userId);
    }

    [Fact]
    public void UserIdShouldRejectMissingClaim()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var currentUser = new CurrentUser(accessor);

        var action = () => currentUser.UserId;

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Authenticated user ID is missing or invalid.");
    }

    [Fact]
    public void UserIdShouldRejectMissingHttpContext()
    {
        var accessor = new HttpContextAccessor();

        var currentUser = new CurrentUser(accessor);

        var action = () => currentUser.UserId;

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Authenticated user ID is missing or invalid.");
    }

    [Fact]
    public void UserIdShouldRejectInvalidClaim()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(
                        System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                        "not-a-guid")
                ]))
        };

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        var currentUser = new CurrentUser(accessor);

        var action = () => currentUser.UserId;

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Authenticated user ID is missing or invalid.");
    }
}
