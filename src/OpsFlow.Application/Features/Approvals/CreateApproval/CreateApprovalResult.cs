using OpsFlow.Domain.Approvals;

namespace OpsFlow.Application.Features.Approvals.CreateApproval;

public sealed record CreateApprovalResult(
    Guid ApprovalId,
    Guid OrganizationId,
    Guid RequesterUserId,
    Guid TimeEntryId,
    string? Comment,
    ApprovalStatus Status,
    DateTimeOffset CreatedAtUtc);
