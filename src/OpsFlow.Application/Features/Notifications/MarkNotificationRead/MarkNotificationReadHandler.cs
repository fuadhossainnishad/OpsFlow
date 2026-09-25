using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;

namespace OpsFlow.Application.Features.Notifications.MarkNotificationRead;

public sealed class MarkNotificationReadHandler(
    ITenantContext tenantContext,
    ICurrentUser currentUser,
    INotificationRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task HandleAsync(
        MarkNotificationReadCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (currentUser.UserId == Guid.Empty)
            throw new UnauthorizedException("Authenticated user is required.");

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        var notification = await repository.GetByIdAsync(
            organizationId,
            currentUser.UserId,
            command.NotificationId,
            cancellationToken);

        if (notification is null)
            throw new NotFoundException("Notification was not found.");

        notification.MarkAsRead(DateTimeOffset.UtcNow);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
