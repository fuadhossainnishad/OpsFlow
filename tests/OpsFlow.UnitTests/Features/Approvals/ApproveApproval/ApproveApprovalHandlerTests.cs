using FluentAssertions;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Approvals;
using OpsFlow.Application.Features.Approvals.ApproveApproval;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.UnitTests.Features.Approvals.ApproveApproval;

public sealed class ApproveApprovalHandlerTests
{
    [Fact]
    public async Task HandleShouldApproveApprovalAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var requesterUserId = Guid.NewGuid();
        var approverUserId = Guid.NewGuid();
        var approvalId = Guid.NewGuid();

        var approval = ApprovalRequest.Create(
            organizationId,
            requesterUserId,
            Guid.NewGuid(),
            "Please review this entry.");

        SetEntityId(approval, approvalId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ApproveApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(approverUserId),
            new FakeApprovalRepository
            {
                Approval = approval
            },
            unitOfWork);

        await handler.HandleAsync(
            new ApproveApprovalCommand(approvalId, "Approved after review."),
            CancellationToken.None);

        approval.Status.Should().Be(ApprovalStatus.Approved);
        approval.DecidedByUserId.Should().Be(approverUserId);
        approval.DecisionComment.Should().Be("Approved after review.");
        approval.DecidedAtUtc.Should().NotBeNull();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectWhenApprovalDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new ApproveApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeApprovalRepository(),
            unitOfWork);

        var action = () => handler.HandleAsync(
            new ApproveApprovalCommand(Guid.NewGuid(), null),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Approval was not found.");

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenApprovalIsAlreadyDecided()
    {
        var organizationId = Guid.NewGuid();
        var requesterUserId = Guid.NewGuid();
        var approverUserId = Guid.NewGuid();
        var approvalId = Guid.NewGuid();

        var approval = ApprovalRequest.Create(
            organizationId,
            requesterUserId,
            Guid.NewGuid(),
            null);

        approval.Approve(approverUserId, "Already approved.");
        SetEntityId(approval, approvalId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ApproveApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(approverUserId),
            new FakeApprovalRepository
            {
                Approval = approval
            },
            unitOfWork);

        var action = () => handler.HandleAsync(
            new ApproveApprovalCommand(approvalId, "Second approval."),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("Only pending approvals can be changed.");

        approval.Status.Should().Be(ApprovalStatus.Approved);
        approval.DecisionComment.Should().Be("Already approved.");
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenApprovalWasRejected()
    {
        var organizationId = Guid.NewGuid();
        var requesterUserId = Guid.NewGuid();
        var approverUserId = Guid.NewGuid();
        var approvalId = Guid.NewGuid();

        var approval = ApprovalRequest.Create(
            organizationId,
            requesterUserId,
            Guid.NewGuid(),
            null);

        approval.Reject(approverUserId, "Needs changes.");
        SetEntityId(approval, approvalId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new ApproveApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(approverUserId),
            new FakeApprovalRepository
            {
                Approval = approval
            },
            unitOfWork);

        var action = () => handler.HandleAsync(
            new ApproveApprovalCommand(approvalId, "Approve anyway."),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("Only pending approvals can be changed.");

        approval.Status.Should().Be(ApprovalStatus.Rejected);
        approval.DecisionComment.Should().Be("Needs changes.");
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static void SetEntityId(
        ApprovalRequest approval,
        Guid id)
    {
        var property = typeof(ApprovalRequest)
            .BaseType!
            .GetProperty(
                nameof(OpsFlow.Domain.Common.Entity.Id));

        property!.SetValue(approval, id);
    }

    private sealed class FakeTenantContext(Guid organizationId)
        : ITenantContext
    {
        public Task<Guid> GetOrganizationIdAsync(
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(organizationId);
    }

    private sealed class FakeCurrentUser(Guid userId)
        : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeApprovalRepository
        : IApprovalRepository
    {
        public ApprovalRequest? Approval { get; init; }

        public Task<ApprovalRequest?> GetByIdAsync(
            Guid organizationId,
            Guid approvalId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(Approval);

        public Task AddAsync(
            ApprovalRequest approval,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.CompletedTask;

        public Task<bool> HasPendingForTimeEntryAsync(
            Guid organizationId,
            Guid timeEntryId,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult(false);

        public Task<IReadOnlyList<ApprovalRequest>> ListAsync(
            Guid organizationId,
            Guid? requesterUserId,
            ApprovalStatus? status,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.FromResult<IReadOnlyList<ApprovalRequest>>([]);
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;
            return System.Threading.Tasks.Task.FromResult(1);
        }
    }
}
