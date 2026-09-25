using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Notifications.GetUnreadNotificationCount;

public sealed class GetUnreadNotificationCountHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    INotificationRepository repository)
{
    public async Task<int> HandleAsync(
        GetUnreadNotificationCountQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (currentUser.UserId == Guid.Empty)
            throw new UnauthorizedException("Authenticated user is required.");

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        return await repository.GetUnreadCountAsync(
            organizationId,
            currentUser.UserId,
            cancellationToken);
    }
}
