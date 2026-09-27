using FluentAssertions;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.UnitTests.Domain.Approvals;

public sealed class ApprovalRequestTests
{
    [Fact]
    public void CreateShouldInitializePendingRequest()
    {
        var organizationId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var request = ApprovalRequest.Create(
            organizationId,
            requesterId,
            timeEntryId,
            "  Please review  ");

        request.Id.Should().NotBe(Guid.Empty);
        request.OrganizationId.Should().Be(organizationId);
        request.RequesterUserId.Should().Be(requesterId);
        request.TimeEntryId.Should().Be(timeEntryId);
        request.Comment.Should().Be("Please review");
        request.Status.Should().Be(ApprovalStatus.Pending);
        request.DecidedByUserId.Should().BeNull();
        request.DecisionComment.Should().BeNull();
        request.DecidedAtUtc.Should().BeNull();
    }

    [Fact]
    public void CreateShouldNormalizeBlankCommentToNull()
    {
        var request = ApprovalRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "   ");

        request.Comment.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    public void CreateShouldRejectEmptyOrganizationId(int _)
    {
        var action = () => ApprovalRequest.Create(
            Guid.Empty,
            Guid.NewGuid(),
            Guid.NewGuid(),
            null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Organization ID is required*");
    }

    [Fact]
    public void CreateShouldRejectEmptyRequesterId()
    {
        var action = () => ApprovalRequest.Create(
            Guid.NewGuid(),
            Guid.Empty,
            Guid.NewGuid(),
            null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Requester user ID is required*");
    }

    [Fact]
    public void CreateShouldRejectEmptyTimeEntryId()
    {
        var action = () => ApprovalRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty,
            null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Time entry ID is required*");
    }

    [Fact]
    public void ApproveShouldSetApprovalDecision()
    {
        var request = CreateRequest();
        var deciderId = Guid.NewGuid();

        request.Approve(deciderId, "  Approved for review  ");

        request.Status.Should().Be(ApprovalStatus.Approved);
        request.DecidedByUserId.Should().Be(deciderId);
        request.DecisionComment.Should().Be("Approved for review");
        request.DecidedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void RejectShouldSetRejectionDecision()
    {
        var request = CreateRequest();
        var deciderId = Guid.NewGuid();

        request.Reject(deciderId, "  Missing details  ");

        request.Status.Should().Be(ApprovalStatus.Rejected);
        request.DecidedByUserId.Should().Be(deciderId);
        request.DecisionComment.Should().Be("Missing details");
        request.DecidedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void CancelShouldAllowRequesterToCancel()
    {
        var request = CreateRequest();

        request.Cancel(request.RequesterUserId);

        request.Status.Should().Be(ApprovalStatus.Cancelled);
        request.DecidedByUserId.Should().Be(request.RequesterUserId);
        request.DecidedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void CancelShouldRejectNonRequester()
    {
        var request = CreateRequest();

        var action = () => request.Cancel(Guid.NewGuid());

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*Only the requester can cancel*");

        request.Status.Should().Be(ApprovalStatus.Pending);
    }

    [Fact]
    public void ApproveShouldRejectEmptyDecider()
    {
        var request = CreateRequest();

        var action = () => request.Approve(Guid.Empty, null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Deciding user ID is required*");
    }

    [Fact]
    public void DecidedApprovalShouldNotBeChangeable()
    {
        var request = CreateRequest();

        request.Approve(Guid.NewGuid(), null);

        var approveAgain = () => request.Approve(Guid.NewGuid(), null);
        var reject = () => request.Reject(Guid.NewGuid(), null);
        var cancel = () => request.Cancel(request.RequesterUserId);

        approveAgain.Should().Throw<InvalidOperationException>();
        reject.Should().Throw<InvalidOperationException>();
        cancel.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void RejectShouldNormalizeBlankDecisionCommentToNull()
    {
        var request = CreateRequest();

        request.Reject(Guid.NewGuid(), "   ");

        request.DecisionComment.Should().BeNull();
    }

    [Fact]
    public void RejectShouldRejectEmptyDecider()
    {
        var request = CreateRequest();

        var action = () => request.Reject(Guid.Empty, null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Deciding user ID is required*");

        request.Status.Should().Be(ApprovalStatus.Pending);
    }

    private static ApprovalRequest CreateRequest()
        => ApprovalRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Review this entry");
}
