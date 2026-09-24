using Microsoft.AspNetCore.Authorization;
using OpsFlow.Api.Contracts.Projects;
using OpsFlow.Application.Features.Projects.CreateProject;

namespace OpsFlow.Api.Endpoints.Projects;

public static class CreateProjectEndpoint
{
    public static IEndpointRouteBuilder MapCreateProjectEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/v1/projects",
            [Authorize] async (
                CreateProjectRequest request,
                CreateProjectHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateProjectCommand(
                    request.Name,
                    request.Key,
                    request.Description);

                var result = await handler.HandleAsync(
                    command,
                    cancellationToken);

                var response = new CreateProjectResponse(
                    result.ProjectId,
                    result.OrganizationId,
                    result.Name,
                    result.Key,
                    result.Description,
                    result.Status);

                return Results.Created(
                    $"/api/v1/projects/{result.ProjectId}",
                    response);
            })
        .WithName("CreateProject")
        .WithTags("Projects")
        .Produces<CreateProjectResponse>(
            StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }
}
