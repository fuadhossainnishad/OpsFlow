using Microsoft.AspNetCore.Authorization;
using OpsFlow.Application.Features.Tasks.GetTask;

namespace OpsFlow.Api.Endpoints.Tasks;

public static class GetTaskEndpoint
{
    public static IEndpointRouteBuilder MapGetTaskEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/v1/tasks/{taskId:guid}",
            [Authorize] async (
                Guid taskId,
                  GetTaskHandler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(
                    new GetTaskQuery(taskId),
                    cancellationToken);

                return Results.Ok(result);
            })
        .WithName("GetTask")
        .WithTags("Tasks")
        .Produces<GetTaskResult>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
