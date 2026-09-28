using FluentAssertions;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.TimeTracking;
using OpsFlow.Application.Features.TimeTracking.ListTimeEntries;
using OpsFlow.Application.Features.TimeTracking.UpdateTimeEntry;
using OpsFlow.Domain.Tasks;
using OpsFlow.Domain.TimeTracking;

namespace OpsFlow.UnitTests.Features.TimeTracking.UpdateTimeEntry;

public sealed class UpdateTimeEntryHandlerTests
{
    [Fact]
    public async Task HandleShouldUpdateEntryAndSaveChanges()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var originalTaskId = Guid.NewGuid();
        var newTaskId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var startedAt = DateTimeOffset.UtcNow.AddHours(-3);
        var endedAt = startedAt.AddHours(2);

        var entry = TimeEntry.CreateManual(
            organizationId,
            userId,
            projectId,
            originalTaskId,
            "Old description",
            startedAt,
            startedAt.AddHours(1));

        SetEntityId(entry, timeEntryId);

        var task = TaskItem.Create(
            organizationId,
            projectId,
            "Updated task");

        var repository = new FakeTimeEntryRepository
        {
            Entry = entry
        };

        var taskRepository = new FakeTaskRepository
        {
            Task = task
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateTimeEntryHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(userId),
            repository,
            taskRepository,
            unitOfWork);

        var result = await handler.HandleAsync(
            new UpdateTimeEntryCommand(
                timeEntryId,
                newTaskId,
                "  Updated description  ",
                startedAt,
                endedAt),
            CancellationToken.None);

        result.TimeEntryId.Should().Be(entry.Id);
        result.ProjectId.Should().Be(projectId);
        result.TaskId.Should().Be(newTaskId);
        result.Description.Should().Be("Updated description");
        result.StartedAtUtc.Should().Be(startedAt);
        result.EndedAtUtc.Should().Be(endedAt);
        result.DurationSeconds.Should().Be(7200);
        result.IsManual.Should().BeTrue();

        entry.TaskId.Should().Be(newTaskId);
        entry.Description.Should().Be("Updated description");
        entry.StartedAtUtc.Should().Be(startedAt);
        entry.EndedAtUtc.Should().Be(endedAt);
        entry.DurationSeconds.Should().Be(7200);

        taskRepository.GetByIdCallCount.Should().Be(1);
        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldUpdateRunningEntryWithoutTask()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var startedAt = DateTimeOffset.UtcNow.AddHours(-2);

        var entry = TimeEntry.Start(
            organizationId,
            userId,
            projectId,
            Guid.NewGuid(),
            "Old description");

        SetEntityId(entry, timeEntryId);

        var repository = new FakeTimeEntryRepository
        {
            Entry = entry
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateTimeEntryHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(userId),
            repository,
            new FakeTaskRepository(),
            unitOfWork);

        var result = await handler.HandleAsync(
            new UpdateTimeEntryCommand(
                timeEntryId,
                null,
                "Running entry",
                startedAt,
                null),
            CancellationToken.None);

        result.TimeEntryId.Should().Be(timeEntryId);
        result.TaskId.Should().BeNull();
        result.Description.Should().Be("Running entry");
        result.StartedAtUtc.Should().Be(startedAt);
        result.EndedAtUtc.Should().BeNull();
        result.DurationSeconds.Should().BeNull();
        result.IsManual.Should().BeFalse();

        entry.IsRunning.Should().BeTrue();
        entry.TaskId.Should().BeNull();
        entry.DurationSeconds.Should().BeNull();

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectWhenEntryDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var repository = new FakeTimeEntryRepository();

        var handler = CreateHandler(
            organizationId,
            Guid.NewGuid(),
            repository);

        var action = () => handler.HandleAsync(
            new UpdateTimeEntryCommand(
                Guid.NewGuid(),
                null,
                "Description",
                DateTimeOffset.UtcNow.AddHours(-1),
                null),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Time entry was not found.");

        repository.GetByIdCallCount.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldRejectWhenTaskDoesNotExist()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var entry = TimeEntry.Start(
            organizationId,
            userId,
            projectId,
            null,
            "Description");

        SetEntityId(entry, timeEntryId);

        var repository = new FakeTimeEntryRepository
        {
            Entry = entry
        };

        var taskRepository = new FakeTaskRepository();

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateTimeEntryHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(userId),
            repository,
            taskRepository,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new UpdateTimeEntryCommand(
                timeEntryId,
                taskId,
                "Updated",
                DateTimeOffset.UtcNow.AddHours(-1),
                null),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<NotFoundException>();

        exception.Which.Message
            .Should()
            .Be("Task was not found.");

        taskRepository.GetByIdCallCount.Should().Be(1);
        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectWhenTaskBelongsToAnotherProject()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var otherProjectId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var entry = TimeEntry.Start(
            organizationId,
            userId,
            projectId,
            null,
            "Description");

        SetEntityId(entry, timeEntryId);

        var task = TaskItem.Create(
            organizationId,
            otherProjectId,
            "Other project task");

        var repository = new FakeTimeEntryRepository
        {
            Entry = entry
        };

        var taskRepository = new FakeTaskRepository
        {
            Task = task
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateTimeEntryHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(userId),
            repository,
            taskRepository,
            unitOfWork);

        var action = () => handler.HandleAsync(
            new UpdateTimeEntryCommand(
                timeEntryId,
                task.Id,
                "Updated",
                DateTimeOffset.UtcNow.AddHours(-1),
                null),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ConflictException>();

        exception.Which.Message
            .Should()
            .Be("The task does not belong to the time entry project.");

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldPropagateInvalidEndTime()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        var startedAt = DateTimeOffset.UtcNow;

        var entry = TimeEntry.Start(
            organizationId,
            userId,
            projectId,
            null,
            "Description");

        SetEntityId(entry, timeEntryId);

        var repository = new FakeTimeEntryRepository
        {
            Entry = entry
        };

        var unitOfWork = new FakeUnitOfWork();

        var handler = new UpdateTimeEntryHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(userId),
            repository,
            new FakeTaskRepository(),
            unitOfWork);

        var action = () => handler.HandleAsync(
            new UpdateTimeEntryCommand(
                timeEntryId,
                null,
                "Updated",
                startedAt,
                startedAt),
            CancellationToken.None);

        var exception = await action.Should()
            .ThrowAsync<ArgumentException>();

        exception.Which.Message
            .Should()
            .Be("End time must be after start time. (Parameter 'endedAtUtc')");

        unitOfWork.SaveChangesCallCount.Should().Be(0);
    }

    private static UpdateTimeEntryHandler CreateHandler(
        Guid organizationId,
        Guid currentUserId,
        FakeTimeEntryRepository repository)
    {
        return new UpdateTimeEntryHandler(
            new FakeTenantContext(organizationId),
            new FakeCurrentUser(currentUserId),
            repository,
            new FakeTaskRepository(),
            new FakeUnitOfWork());
    }

    private static void SetEntityId(
        TimeEntry entry,
        Guid id)
    {
        var property = typeof(TimeEntry)
            .BaseType!
            .GetProperty(
                nameof(OpsFlow.Domain.Common.Entity.Id));

        property!.SetValue(entry, id);
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

    private sealed class FakeTimeEntryRepository
        : ITimeEntryRepository
    {
        public TimeEntry? Entry { get; init; }

        public int GetByIdCallCount { get; private set; }

        public Task AddAsync(
            TimeEntry timeEntry,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.CompletedTask;

        public Task<TimeEntry?> GetByIdAsync(
            Guid organizationId,
            Guid userId,
            Guid timeEntryId,
            CancellationToken cancellationToken)
        {
            GetByIdCallCount++;
            return Task.FromResult(Entry);
        }

        public Task<IReadOnlyList<TimeEntryRecord>> GetAsync(
            Guid organizationId,
            Guid userId,
            Guid? projectId,
            Guid? taskId,
            DateTimeOffset? fromUtc,
            DateTimeOffset? toUtc,
            int skip,
            int take,
            TimeEntrySortField sortBy,
            TimeEntrySortOrder sortOrder,
            CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<TimeEntryRecord>>([]);

        public Task<bool> HasRunningEntryAsync(
            Guid organizationId,
            Guid userId,
            CancellationToken cancellationToken)
            => Task.FromResult(false);

        public void Remove(TimeEntry timeEntry)
        {
        }
    }

    private sealed class FakeTaskRepository
        : ITaskRepository
    {
        public TaskItem? Task { get; init; }

        public int GetByIdCallCount { get; private set; }

        public Task<TaskItem?> GetByIdAsync(
            Guid organizationId,
            Guid taskId,
            CancellationToken cancellationToken)
        {
            GetByIdCallCount++;
            return System.Threading.Tasks.Task.FromResult(Task);
        }

        public Task AddAsync(
            TaskItem task,
            CancellationToken cancellationToken)
            => System.Threading.Tasks.Task.CompletedTask;
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
