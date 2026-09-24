using Microsoft.AspNetCore.Authorization;
using OpsFlow.Api.Contracts.Organizations;
using OpsFlow.Application.Features.Organizations.CreateOrganization;

namespace OpsFlow.Api.Endpoints.Organizations;

public static class CreateOrganizationEndpoint
{
    public static IEndpointRouteBuilder MapCreateOrganizationEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/v1/organizations",
            [Authorize] async (
                CreateOrganizationRequest request,
                CreateOrganizationHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateOrganizationCommand(
                    request.Name,
                    request.Slug);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                var response = new CreateOrganizationResponse(
                    result.OrganizationId,
                    result.Name,
                    result.Slug);

                return Results.Created(
                    $"/api/v1/organizations/{result.OrganizationId}",
                    response);
            })
        .WithName("CreateOrganization")
        .WithTags("Organizations")
        .Produces<CreateOrganizationResponse>(
            StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }
}
