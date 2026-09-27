using FluentAssertions;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Approvals;
using OpsFlow.Application.Features.Approvals.CancelApproval;
using OpsFlow.Domain.Approvals;

namespace OpsFlow.UnitTests.Features.Approvals.CancelApproval;

public sealed class CancelApprovalHandlerTests
{
    [Fact]
    public async Task HandleShouldCancelApprovalAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var requesterUserId = Guid.NewGuid();
        var approvalId = Guid.NewGuid();

        var approval = ApprovalRequest.Create(
            organizationId,
            requesterUserId,
            Guid.NewGuid(),
            "Please review this entry.");

        SetEntityId(approval, approvalId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(requesterUserId),
            new FakeApprovalRepository
            {
                Approval = approval
            },
            unitOfWork);

        await handler.HandleAsync(
            new CancelApprovalCommand(approvalId),
            CancellationToken.None);

        approval.Status.Should().Be(ApprovalStatus.Cancelled);
        approval.DecidedByUserId.Should().Be(requesterUserId);
        approval.DecidedAtUtc.Should().NotBeNull();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectWhenApprovalDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeApprovalRepository(),
            unitOfWork);

        var action = () => handler.HandleAsync(
            new CancelApprovalCommand(Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Approval was not found.");

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenCurrentUserIsNotRequester()
    {
        var organizationId = Guid.NewGuid();
        var requesterUserId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var approvalId = Guid.NewGuid();

        var approval = ApprovalRequest.Create(
            organizationId,
            requesterUserId,
            Guid.NewGuid(),
            "Approval comment");

        SetEntityId(approval, approvalId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(differentUserId),
            new FakeApprovalRepository
            {
                Approval = approval
            },
            unitOfWork);

        var action = () => handler.HandleAsync(
            new CancelApprovalCommand(approvalId),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("Only the requester can cancel an approval.");

        approval.Status.Should().Be(ApprovalStatus.Pending);
        approval.DecidedByUserId.Should().BeNull();
        approval.DecidedAtUtc.Should().BeNull();

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenApprovalIsAlreadyDecided()
    {
        var organizationId = Guid.NewGuid();
        var requesterUserId = Guid.NewGuid();
        var approvalId = Guid.NewGuid();

        var approval = ApprovalRequest.Create(
            organizationId,
            requesterUserId,
            Guid.NewGuid(),
            null);

        approval.Approve(Guid.NewGuid(), "Approved");

        SetEntityId(approval, approvalId);

        var unitOfWork = new FakeUnitOfWork();

        var handler = new CancelApprovalHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(requesterUserId),
            new FakeApprovalRepository
            {
                Approval = approval
            },
            unitOfWork);

        var action = () => handler.HandleAsync(
            new CancelApprovalCommand(approvalId),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("Only pending approvals can be changed.");

        approval.Status.Should().Be(ApprovalStatus.Approved);
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
