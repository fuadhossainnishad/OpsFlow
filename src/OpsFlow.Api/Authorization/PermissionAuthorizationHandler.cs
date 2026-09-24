using Microsoft.AspNetCore.Authorization;
using OpsFlow.Application.Abstractions.Authorization;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Tenancy;

namespace OpsFlow.Api.Authorization;

public sealed class PermissionAuthorizationHandler(
    IPermissionChecker permissionChecker,
    ICurrentUser currentUser,
    ITenantContext tenantContext)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return;
        }

        var organizationId = await tenantContext.GetOrganizationIdAsync(
            context.Resource is HttpContext httpContext
                ? httpContext.RequestAborted
                : CancellationToken.None);

        var hasPermission = await permissionChecker.HasPermissionAsync(
            currentUser.UserId,
            organizationId,
            requirement.PermissionCode,
            context.Resource is HttpContext requestContext
                ? requestContext.RequestAborted
                : CancellationToken.None);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}
