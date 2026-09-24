using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Identity;
using OpsFlow.Application.Features.Identity.Login;

namespace OpsFlow.Api.Endpoints.Identity;

public static class LoginUserEndpoint
{
    public static IEndpointRouteBuilder MapLoginUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/v1/auth/login",
            async (
                LoginUserRequest request,
                LoginUserHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new LoginUserCommand(
                    request.Email,
                    request.Password);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                var response = new LoginUserResponse(
                    result.UserId,
                    result.AccessToken,
                    result.RefreshToken);

                return Results.Ok(response);
            })
        .WithName("LoginUser")
        .WithTags("Authentication")
        .Produces<LoginUserResponse>(
            StatusCodes.Status200OK)
        .Produces<ProblemDetails>(
            StatusCodes.Status401Unauthorized)
        .Produces<ProblemDetails>(
            StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}
