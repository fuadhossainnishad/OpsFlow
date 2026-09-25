using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OpsFlow.Application.Features.Notifications.GetUnreadNotificationCount;
using OpsFlow.Application.Features.Notifications.ListNotifications;
using OpsFlow.Application.Features.Notifications.MarkNotificationRead;
using OpsFlow.Application.Authorization;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public sealed class NotificationsController(
    ListNotificationsHandler listNotificationsHandler,
    GetUnreadNotificationCountHandler unreadCountHandler,
    MarkNotificationReadHandler markNotificationReadHandler) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionCodes.NotificationsRead)]
    public async Task<ActionResult<ListNotificationsResult>> List(
        [FromQuery] bool? unreadOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await listNotificationsHandler.HandleAsync(
            new ListNotificationsQuery(unreadOnly, page, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("unread-count")]
    [Authorize(Policy = PermissionCodes.NotificationsRead)]
    public async Task<ActionResult<int>> GetUnreadCount(
        CancellationToken cancellationToken)
    {
        return Ok(await unreadCountHandler.HandleAsync(
            new GetUnreadNotificationCountQuery(),
            cancellationToken));
    }

    [HttpPost("{notificationId:guid}/read")]
    [Authorize(Policy = PermissionCodes.NotificationsRead)]
    public async Task<IActionResult> MarkRead(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        await markNotificationReadHandler.HandleAsync(
            new MarkNotificationReadCommand(notificationId),
            cancellationToken);

        return NoContent();
    }
}
