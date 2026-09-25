using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Api.Contracts.Approvals;
using OpsFlow.Application.Authorization;
using OpsFlow.Application.Features.Approvals.ApproveApproval;
using OpsFlow.Application.Features.Approvals.CancelApproval;
using OpsFlow.Application.Features.Approvals.CreateApproval;
using OpsFlow.Application.Features.Approvals.GetApproval;
using OpsFlow.Application.Features.Approvals.ListApprovals;
using OpsFlow.Application.Features.Approvals.RejectApproval;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.Api.Controllers;

[ApiController]
[Route("api/v1/approvals")]
[Authorize]
public sealed class ApprovalsController(
    CreateApprovalHandler createApprovalHandler,
    ListApprovalsHandler listApprovalsHandler,
    GetApprovalHandler getApprovalHandler,
    ApproveApprovalHandler approveApprovalHandler,
    RejectApprovalHandler rejectApprovalHandler,
    CancelApprovalHandler cancelApprovalHandler) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = PermissionCodes.ApprovalsCreate)]
    public async Task<ActionResult<CreateApprovalResult>> Create(
        CreateApprovalRequest request,
        CancellationToken cancellationToken)
        => Ok(await createApprovalHandler.HandleAsync(
            new CreateApprovalCommand(request.TimeEntryId, request.Comment),
            cancellationToken));

    [HttpGet]
    [Authorize(Policy = PermissionCodes.ApprovalsRead)]
    public async Task<ActionResult<ListApprovalsResult>> List(
        [FromQuery] Guid? requesterUserId,
        [FromQuery] ApprovalStatus? status,
        CancellationToken cancellationToken)
        => Ok(await listApprovalsHandler.HandleAsync(
            new ListApprovalsQuery(requesterUserId, status),
            cancellationToken));

    [HttpGet("{approvalId:guid}")]
    [Authorize(Policy = PermissionCodes.ApprovalsRead)]
    public async Task<ActionResult<GetApprovalResult>> Get(
        Guid approvalId,
        CancellationToken cancellationToken)
        => Ok(await getApprovalHandler.HandleAsync(
            new GetApprovalQuery(approvalId),
            cancellationToken));

    [HttpPost("{approvalId:guid}/approve")]
    [Authorize(Policy = PermissionCodes.ApprovalsApprove)]
    public async Task<IActionResult> Approve(
        Guid approvalId,
        ApprovalDecisionRequest request,
        CancellationToken cancellationToken)
    {
        await approveApprovalHandler.HandleAsync(
            new ApproveApprovalCommand(approvalId, request.Comment),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{approvalId:guid}/reject")]
    [Authorize(Policy = PermissionCodes.ApprovalsReject)]
    public async Task<IActionResult> Reject(
        Guid approvalId,
        ApprovalDecisionRequest request,
        CancellationToken cancellationToken)
    {
        await rejectApprovalHandler.HandleAsync(
            new RejectApprovalCommand(approvalId, request.Comment),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{approvalId:guid}/cancel")]
    [Authorize(Policy = PermissionCodes.ApprovalsCancel)]
    public async Task<IActionResult> Cancel(
        Guid approvalId,
        CancellationToken cancellationToken)
    {
        await cancelApprovalHandler.HandleAsync(
            new CancelApprovalCommand(approvalId),
            cancellationToken);

        return NoContent();
    }
}
