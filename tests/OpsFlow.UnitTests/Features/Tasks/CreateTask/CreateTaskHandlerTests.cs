using FluentAssertions;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.CreateTask;
using OpsFlow.Domain.Projects;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.UnitTests.Features.Tasks.CreateTask;

public sealed class CreateTaskHandlerTests
{
    [Fact]
    public async Task HandleShouldCreateTaskAuditOutboxAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var assigneeUserId = Guid.NewGuid();

        var project = Project.Create(
            organizationId,
            "Operations",
            "OPS",
            "Operations project");

        var projectRepository = new FakeProjectRepository
        {
            Project = project
        };

        var membershipRepository = new FakeMembershipRepository
        {
            IsActiveMember = true
        };

        var taskRepository = new FakeTaskRepository();
        var auditLogger = new FakeAuditLogger();
        var outboxWriter = new FakeOutboxWriter();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateTaskHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(currentUserId),
            projectRepository,
            membershipRepository,
            taskRepository,
            auditLogger,
            unitOfWork,
            outboxWriter);

        var result = await handler.HandleAsync(
            new CreateTaskCommand(
                project.Id,
                "  Fix inventory sync  ",
                "  Synchronize stock records.  ",
                assigneeUserId),
            CancellationToken.None);

        result.TaskId.Should().NotBeEmpty();
        result.OrganizationId.Should().Be(organizationId);
        result.ProjectId.Should().Be(project.Id);
        result.Title.Should().Be("Fix inventory sync");
        result.Description.Should().Be("Synchronize stock records.");
        result.AssigneeUserId.Should().Be(assigneeUserId);
        result.Status.Should().Be(OpsFlow.Domain.Tasks.TaskStatus.Todo);
        result.RowVersion.Should().Be(Convert.ToBase64String(taskRepository.AddedTask!.RowVersion));

        taskRepository.AddCallCount.Should().Be(1);
        taskRepository.AddedTask.Should().NotBeNull();

        var task = taskRepository.AddedTask!;
        task.Id.Should().Be(result.TaskId);
        task.OrganizationId.Should().Be(organizationId);
        task.ProjectId.Should().Be(project.Id);
        task.Title.Should().Be("Fix inventory sync");
        task.Description.Should().Be("Synchronize stock records.");
        task.AssigneeUserId.Should().Be(assigneeUserId);
        task.Status.Should().Be(OpsFlow.Domain.Tasks.TaskStatus.Todo);

        membershipRepository.IsActiveMemberCallCount.Should().Be(1);

        auditLogger.CallCount.Should().Be(1);
        auditLogger.OrganizationId.Should().Be(organizationId);
        auditLogger.ActorUserId.Should().Be(currentUserId);
        auditLogger.Action.Should().Be("task.created");
        auditLogger.Resource.Should().Be("task");
        auditLogger.ResourceId.Should().Be(task.Id);
        auditLogger.BeforeJson.Should().BeNull();
        auditLogger.AfterJson.Should().NotBeNull();
        auditLogger.AfterJson.Should().Contain(task.Id.ToString());
        auditLogger.AfterJson.Should().Contain(project.Id.ToString());
        auditLogger.AfterJson.Should().Contain("Fix inventory sync");
        auditLogger.AfterJson.Should().Contain(assigneeUserId.ToString());
        auditLogger.AfterJson.Should().Contain("\"Status\":\"Todo\"");

        outboxWriter.CallCount.Should().Be(1);
        outboxWriter.MessageType.Should().Be("task.created");
        outboxWriter.Message.Should().NotBeNull();
        outboxWriter.Message.Should().BeOfType<OpsFlow.Contracts.Events.TaskCreatedEvent>();

        var eventMessage =
            (OpsFlow.Contracts.Events.TaskCreatedEvent)outboxWriter.Message!;

        eventMessage.TaskId.Should().Be(task.Id);
        eventMessage.OrganizationId.Should().Be(organizationId);
        eventMessage.ProjectId.Should().Be(project.Id);
        eventMessage.Title.Should().Be("Fix inventory sync");
        eventMessage.AssigneeUserId.Should().Be(assigneeUserId);

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectNullCommand()
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            null!,
            CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task HandleShouldRejectWhenProjectDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var taskRepository = new FakeTaskRepository();
        var auditLogger = new FakeAuditLogger();
        var outboxWriter = new FakeOutboxWriter();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateTaskHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeProjectRepository(),
            new FakeMembershipRepository(),
            taskRepository,
            auditLogger,
            unitOfWork,
            outboxWriter);

        var action = () => handler.HandleAsync(
            new CreateTaskCommand(
                Guid.NewGuid(),
                "Task",
                null,
                null),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should().Be("Project was not found.");

        taskRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        outboxWriter.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenAssigneeIsNotActiveMember()
    {
        var organizationId = Guid.NewGuid();
        var project = Project.Create(
            organizationId,
            "Operations",
            "OPS",
            null);

        var taskRepository = new FakeTaskRepository();
        var auditLogger = new FakeAuditLogger();
        var outboxWriter = new FakeOutboxWriter();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateTaskHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeProjectRepository
            {
                Project = project
            },
            new FakeMembershipRepository
            {
                IsActiveMember = false
            },
            taskRepository,
            auditLogger,
            unitOfWork,
            outboxWriter);

        var action = () => handler.HandleAsync(
            new CreateTaskCommand(
                project.Id,
                "Task",
                null,
                Guid.NewGuid()),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should().Be(
                "The assignee is not an active member of this organization.");

        taskRepository.AddCallCount.Should().Be(0);
        auditLogger.CallCount.Should().Be(0);
        outboxWriter.CallCount.Should().Be(0);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static CreateTaskHandler CreateHandler()
    {
        return new CreateTaskHandler(
            new FakeTenantContext(Guid.NewGuid()),
            new FakeCurrentUser(Guid.NewGuid()),
            new FakeProjectRepository(),
            new FakeMembershipRepository(),
            new FakeTaskRepository(),
            new FakeAuditLogger(),
            new FakeUnitOfWork(),
            new FakeOutboxWriter());
    }

    private sealed class FakeTenantContext(Guid organizationId)
        : ITenantContext
    {
        public Task<Guid> GetOrganizationIdAsync(
            CancellationToken cancellationToken)
            => Task.FromResult(organizationId);
    }

    private sealed class FakeCurrentUser(Guid userId)
        : ICurrentUser
    {
        public Guid UserId => userId;
    }

    private sealed class FakeProjectRepository : IProjectRepository
    {
        public Project? Project { get; init; }

        public Task<bool> ExistsByKeyAsync(
            Guid organizationId,
            string key,
            CancellationToken cancellationToken)
            => Task.FromResult(false);

        public Task<Project?> GetByIdAsync(
            Guid organizationId,
            Guid projectId,
            CancellationToken cancellationToken)
            => Task.FromResult(Project);

        public Task AddAsync(
            Project project,
            CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<IReadOnlyList<Project>> GetAllAsync(
            Guid organizationId,
            CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<Project>>([]);
    }

    private sealed class FakeMembershipRepository : IMembershipRepository
    {
        public bool IsActiveMember { get; init; }
        public int IsActiveMemberCallCount { get; private set; }

        public Task AddAsync(
            OpsFlow.Domain.Organizations.Membership membership,
            CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<OpsFlow.Domain.Organizations.Membership?> GetByIdAsync(
            Guid organizationId,
            Guid membershipId,
            CancellationToken cancellationToken)
            => Task.FromResult<OpsFlow.Domain.Organizations.Membership?>(
                null);

        public Task<IReadOnlyList<OrganizationMemberRecord>>
            GetOrganizationMembersAsync(
                Guid organizationId,
                CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<OrganizationMemberRecord>>([]);

        public Task<bool> IsActiveMemberAsync(
            Guid organizationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            IsActiveMemberCallCount++;
            return Task.FromResult(IsActiveMember);
        }
    }

    private sealed class FakeTaskRepository : ITaskRepository
    {
        public int AddCallCount { get; private set; }
        public TaskItem? AddedTask { get; private set; }

        public Task<TaskItem?> GetByIdAsync(
            Guid organizationId,
            Guid taskId,
            CancellationToken cancellationToken)
            => Task.FromResult<TaskItem?>(null);

        public Task AddAsync(
            TaskItem task,
            CancellationToken cancellationToken)
        {
            AddCallCount++;
            AddedTask = task;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAuditLogger : IAuditLogger
    {
        public int CallCount { get; private set; }
        public Guid OrganizationId { get; private set; }
        public Guid? ActorUserId { get; private set; }
        public string? Action { get; private set; }
        public string? Resource { get; private set; }
        public Guid? ResourceId { get; private set; }
        public string? BeforeJson { get; private set; }
        public string? AfterJson { get; private set; }

        public Task LogAsync(
            Guid organizationId,
            Guid? actorUserId,
            string action,
            string resource,
            Guid? resourceId,
            string? beforeJson,
            string? afterJson,
            CancellationToken cancellationToken)
        {
            CallCount++;
            OrganizationId = organizationId;
            ActorUserId = actorUserId;
            Action = action;
            Resource = resource;
            ResourceId = resourceId;
            BeforeJson = beforeJson;
            AfterJson = afterJson;

            return Task.CompletedTask;
        }
    }

    private sealed class FakeOutboxWriter : IOutboxWriter
    {
        public int CallCount { get; private set; }
        public string? MessageType { get; private set; }
        public object? Message { get; private set; }

        public Task AddAsync(
            string messageType,
            object message,
            CancellationToken cancellationToken)
        {
            CallCount++;
            MessageType = messageType;
            Message = message;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
