using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Approvals;

public sealed class ApprovalRequest : Entity
{
    private ApprovalRequest() { }

    private ApprovalRequest(
        Guid id,
        Guid organizationId,
        Guid requesterUserId,
        Guid timeEntryId,
        string? comment)
    {
        Id = id;
        OrganizationId = organizationId;
        RequesterUserId = requesterUserId;
        TimeEntryId = timeEntryId;
        Comment = Normalize(comment);
        Status = ApprovalStatus.Pending;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid OrganizationId { get; private init; }
    public Guid RequesterUserId { get; private init; }
    public Guid TimeEntryId { get; private init; }
    public string? Comment { get; private set; }
    public ApprovalStatus Status { get; private set; }
    public Guid? DecidedByUserId { get; private set; }
    public string? DecisionComment { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private init; }
    public DateTimeOffset? DecidedAtUtc { get; private set; }

    public static ApprovalRequest Create(
        Guid organizationId,
        Guid requesterUserId,
        Guid timeEntryId,
        string? comment)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required.", nameof(organizationId));
        if (requesterUserId == Guid.Empty)
            throw new ArgumentException("Requester user ID is required.", nameof(requesterUserId));
        if (timeEntryId == Guid.Empty)
            throw new ArgumentException("Time entry ID is required.", nameof(timeEntryId));

        return new ApprovalRequest(
            Guid.NewGuid(),
            organizationId,
            requesterUserId,
            timeEntryId,
            comment);
    }

    public void Approve(Guid decidedByUserId, string? comment)
    {
        EnsurePending();
        EnsureDecider(decidedByUserId);

        Status = ApprovalStatus.Approved;
        DecidedByUserId = decidedByUserId;
        DecisionComment = Normalize(comment);
        DecidedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Reject(Guid decidedByUserId, string? comment)
    {
        EnsurePending();
        EnsureDecider(decidedByUserId);

        Status = ApprovalStatus.Rejected;
        DecidedByUserId = decidedByUserId;
        DecisionComment = Normalize(comment);
        DecidedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Cancel(Guid userId)
    {
        EnsurePending();

        if (userId != RequesterUserId)
            throw new InvalidOperationException(
                "Only the requester can cancel an approval.");

        Status = ApprovalStatus.Cancelled;
        DecidedByUserId = userId;
        DecidedAtUtc = DateTimeOffset.UtcNow;
    }

    private void EnsurePending()
    {
        if (Status != ApprovalStatus.Pending)
            throw new InvalidOperationException(
                "Only pending approvals can be changed.");
    }

    private static void EnsureDecider(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Deciding user ID is required.", nameof(userId));
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
