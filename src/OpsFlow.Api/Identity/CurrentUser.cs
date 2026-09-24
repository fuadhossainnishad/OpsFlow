using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using OpsFlow.Application.Abstractions.Identity;

namespace OpsFlow.Api.Identity;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var userIdValue = httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                throw new InvalidOperationException(
                    "Authenticated user ID is missing or invalid.");
            }

            return userId;
        }
    }
}
