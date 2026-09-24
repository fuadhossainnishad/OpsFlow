using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Identity;
using OpsFlow.Application.Features.Identity.RegisterUser;

namespace OpsFlow.Api.Endpoints.Identity;

public static class RegisterUserEndpoint
{
    public static IEndpointRouteBuilder MapRegisterUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/v1/auth/register",
            async (
                RegisterUserRequest request,
                RegisterUserHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RegisterUserCommand(
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.Password);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                var response = new RegisterUserResponse(
                    result.UserId,
                    result.Email,
                    result.FirstName,
                    result.LastName);

                return Results.Created(
                    $"/api/v1/users/{result.UserId}",
                    response);
            })
        .WithName("RegisterUser")
        .WithTags("Authentication")
        .Produces<RegisterUserResponse>(
            StatusCodes.Status201Created)
        .Produces<ProblemDetails>(
            StatusCodes.Status409Conflict)
        .Produces<ProblemDetails>(
            StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}
