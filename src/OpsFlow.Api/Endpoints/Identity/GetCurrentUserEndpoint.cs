using Microsoft.AspNetCore.Authorization;
using OpsFlow.Application.Abstractions.Identity;

namespace OpsFlow.Api.Endpoints.Identity;

public static class GetCurrentUserEndpoint
{
    public static IEndpointRouteBuilder MapGetCurrentUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/v1/auth/me",
            [Authorize] (ICurrentUser currentUser) =>
            {
                return Results.Ok(new
                {
                    currentUser.UserId
                });
            })
        .WithName("GetCurrentUser")
        .WithTags("Authentication")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        return endpoints;
    }
}
