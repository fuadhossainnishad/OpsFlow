using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Organizations;
using OpsFlow.Application.Features.Organizations.CreateOrganization;
using OpsFlow.Application.Authorization;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/organizations")]
public sealed class OrganizationsController(
    CreateOrganizationHandler createOrganizationHandler) : ControllerBase
{
    [Authorize(Policy = PermissionCodes.OrganizationsCreate)]
    [HttpPost]
    [ProducesResponseType(
        typeof(CreateOrganizationResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateOrganizationResponse>> Create(
        CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(
            request.Name,
            request.Slug);

        var result = await createOrganizationHandler.HandleAsync(
            command,
            cancellationToken);

        var response = new CreateOrganizationResponse(
            result.OrganizationId,
            result.Name,
            result.Slug);

        return Created(
            $"/api/v1/organizations/{result.OrganizationId}",
            response);
    }
}
