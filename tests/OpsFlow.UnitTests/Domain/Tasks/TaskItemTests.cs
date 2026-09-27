using FluentAssertions;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.UnitTests.Domain.Tasks;

public sealed class TaskItemTests
{
    [Fact]
    public void CreateShouldNormalizeValues()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  Test task  ",
            "  Description  ");

        task.Title.Should().Be("Test task");
        task.Description.Should().Be("Description");
        task.Status.Should().Be(OpsFlow.Domain.Tasks.TaskStatus.Todo);
        task.UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public void CreateShouldConvertBlankDescriptionToNull()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test task",
            "   ");

        task.Description.Should().BeNull();
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => TaskItem.Create(
            Guid.Empty,
            Guid.NewGuid(),
            "Task");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectEmptyProjectId()
    {
        var action = () => TaskItem.Create(
            Guid.NewGuid(),
            Guid.Empty,
            "Task");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectBlankTitle()
    {
        var action = () => TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "   ");

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AssignToShouldAssignUserAndTouch()
    {
        var task = CreateTask();
        var userId = Guid.NewGuid();

        task.AssignTo(userId);

        task.AssigneeUserId.Should().Be(userId);
        task.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void AssignToShouldRejectEmptyUserId()
    {
        var task = CreateTask();

        var action = () => task.AssignTo(Guid.Empty);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UnassignShouldClearAssignee()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Task",
            assigneeUserId: Guid.NewGuid());

        task.Unassign();

        task.AssigneeUserId.Should().BeNull();
        task.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void ChangeStatusShouldIgnoreSameStatus()
    {
        var task = CreateTask();

        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Todo);

        task.Status.Should().Be(OpsFlow.Domain.Tasks.TaskStatus.Todo);
        task.UpdatedAtUtc.Should().BeNull();
    }

    [Theory]
    [InlineData(OpsFlow.Domain.Tasks.TaskStatus.InProgress)]
    [InlineData(OpsFlow.Domain.Tasks.TaskStatus.Cancelled)]
    public void TodoShouldAllowValidTransitions(OpsFlow.Domain.Tasks.TaskStatus status)
    {
        var task = CreateTask();

        task.ChangeStatus(status);

        task.Status.Should().Be(status);
        task.UpdatedAtUtc.Should().NotBeNull();
    }

    [Theory]
    [InlineData(OpsFlow.Domain.Tasks.TaskStatus.Todo)]
    [InlineData(OpsFlow.Domain.Tasks.TaskStatus.Done)]
    [InlineData(OpsFlow.Domain.Tasks.TaskStatus.Cancelled)]
    public void InProgressShouldAllowValidTransitions(OpsFlow.Domain.Tasks.TaskStatus status)
    {
        var task = CreateTask();

        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.InProgress);
        task.ChangeStatus(status);

        task.Status.Should().Be(status);
    }

    [Fact]
    public void DoneShouldAllowReturningToInProgress()
    {
        var task = CreateTask();

        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.InProgress);
        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Done);
        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.InProgress);

        task.Status.Should().Be(OpsFlow.Domain.Tasks.TaskStatus.InProgress);
    }

    [Fact]
    public void CancelledShouldAllowReturningToTodo()
    {
        var task = CreateTask();

        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Cancelled);
        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Todo);

        task.Status.Should().Be(OpsFlow.Domain.Tasks.TaskStatus.Todo);
    }

    [Fact]
    public void TodoShouldRejectInvalidTransitionToDone()
    {
        var task = CreateTask();

        var action = () => task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Done);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void DoneShouldRejectTransitionToTodo()
    {
        var task = CreateTask();

        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.InProgress);
        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Done);

        var action = () => task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Todo);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CancelledShouldRejectTransitionToDone()
    {
        var task = CreateTask();

        task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Cancelled);

        var action = () => task.ChangeStatus(OpsFlow.Domain.Tasks.TaskStatus.Done);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UpdateShouldNormalizeValues()
    {
        var task = CreateTask();

        task.Update("  Updated title  ", "  Updated description  ");

        task.Title.Should().Be("Updated title");
        task.Description.Should().Be("Updated description");
        task.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void UpdateShouldConvertBlankDescriptionToNull()
    {
        var task = CreateTask();

        task.Update("Updated", "   ");

        task.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateShouldRejectBlankTitle()
    {
        var task = CreateTask();

        var action = () => task.Update("   ", null);

        action.Should().Throw<ArgumentException>();
    }

    private static TaskItem CreateTask() =>
        TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test task");
}
