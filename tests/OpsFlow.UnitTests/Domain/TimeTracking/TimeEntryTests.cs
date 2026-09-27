using FluentAssertions;
using OpsFlow.Domain.TimeTracking;

namespace OpsFlow.UnitTests.Domain.TimeTracking;

public sealed class TimeEntryTests
{
    private static readonly Guid OrganizationId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ProjectId = Guid.NewGuid();

    [Fact]
    public void StartShouldCreateRunningEntry()
    {
        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            Guid.NewGuid(),
            "  Working  ");

        entry.IsRunning.Should().BeTrue();
        entry.Description.Should().Be("Working");
        entry.EndedAtUtc.Should().BeNull();
        entry.DurationSeconds.Should().BeNull();
        entry.IsManual.Should().BeFalse();
    }

    [Fact]
    public void StartShouldRejectEmptyOrganizationId()
    {
        var action = () => TimeEntry.Start(
            Guid.Empty,
            UserId,
            ProjectId,
            null,
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StartShouldRejectEmptyUserId()
    {
        var action = () => TimeEntry.Start(
            OrganizationId,
            Guid.Empty,
            ProjectId,
            null,
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StartShouldRejectEmptyProjectId()
    {
        var action = () => TimeEntry.Start(
            OrganizationId,
            UserId,
            Guid.Empty,
            null,
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StartShouldNormalizeBlankDescription()
    {
        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            "   ");

        entry.Description.Should().BeNull();
    }

    [Fact]
    public void CreateManualShouldCalculateDuration()
    {
        var start = DateTimeOffset.UtcNow.AddHours(-2);
        var end = start.AddMinutes(90);

        var entry = TimeEntry.CreateManual(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            "  Manual work  ",
            start,
            end);

        entry.IsRunning.Should().BeFalse();
        entry.IsManual.Should().BeTrue();
        entry.DurationSeconds.Should().Be(5400);
        entry.Description.Should().Be("Manual work");
    }

    [Fact]
    public void CreateManualShouldRejectEndBeforeStart()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddMinutes(-1);

        var action = () => TimeEntry.CreateManual(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null,
            start,
            end);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void StopShouldCalculateDuration()
    {
        var start = DateTimeOffset.UtcNow;

        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null);

        entry.Update(null, null, start, null);

        var end = start.AddMinutes(30);
        entry.Stop(end);

        entry.IsRunning.Should().BeFalse();
        entry.DurationSeconds.Should().Be(1800);
        entry.EndedAtUtc.Should().Be(end);
        entry.UpdatedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void StopShouldRejectAlreadyStoppedEntry()
    {
        var start = DateTimeOffset.UtcNow;

        var entry = TimeEntry.CreateManual(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null,
            start,
            start.AddMinutes(10));

        var action = () => entry.Stop(start.AddMinutes(20));

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void StopShouldRejectEndBeforeStart()
    {
        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null);

        var action = () => entry.Stop(
            entry.StartedAtUtc.AddSeconds(-1));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateShouldCalculateDuration()
    {
        var start = DateTimeOffset.UtcNow;
        var end = start.AddMinutes(45);
        var taskId = Guid.NewGuid();

        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null);

        entry.Update(
            taskId,
            "  Updated  ",
            start,
            end);

        entry.TaskId.Should().Be(taskId);
        entry.Description.Should().Be("Updated");
        entry.DurationSeconds.Should().Be(2700);
        entry.IsRunning.Should().BeFalse();
    }

    [Fact]
    public void UpdateShouldAllowRunningEntry()
    {
        var start = DateTimeOffset.UtcNow;

        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null);

        entry.Update(
            null,
            "Updated",
            start,
            null);

        entry.IsRunning.Should().BeTrue();
        entry.DurationSeconds.Should().BeNull();
    }

    [Fact]
    public void UpdateShouldRejectInvalidEndTime()
    {
        var start = DateTimeOffset.UtcNow;

        var entry = TimeEntry.Start(
            OrganizationId,
            UserId,
            ProjectId,
            null,
            null);

        var action = () => entry.Update(
            null,
            null,
            start,
            start.AddSeconds(-1));

        action.Should().Throw<ArgumentException>();
    }
}
