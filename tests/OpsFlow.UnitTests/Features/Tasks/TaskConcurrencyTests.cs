using System.Reflection;
using FluentAssertions;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Tasks.Common;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.UnitTests.Features.Tasks;

public sealed class TaskConcurrencyTests
{
    [Fact]
    public void EnsureCurrentShouldAcceptMatchingRowVersion()
    {
        var task = CreateTaskWithRowVersion();
        var rowVersion = Convert.ToBase64String(task.RowVersion);

        var action = () => TaskConcurrency.EnsureCurrent(
            task,
            rowVersion);

        action.Should().NotThrow();
    }

    [Fact]
    public void EnsureCurrentShouldRejectMissingRowVersion()
    {
        var task = CreateTaskWithRowVersion();

        var action = () => TaskConcurrency.EnsureCurrent(
            task,
            " ");

        action.Should()
            .Throw<ConflictException>()
            .WithMessage("A task row version is required.");
    }

    [Fact]
    public void EnsureCurrentShouldRejectInvalidBase64()
    {
        var task = CreateTaskWithRowVersion();

        var action = () => TaskConcurrency.EnsureCurrent(
            task,
            "not-valid-base64");

        action.Should()
            .Throw<ConflictException>()
            .WithMessage("The task row version is invalid.");
    }

    [Fact]
    public void EnsureCurrentShouldRejectStaleRowVersion()
    {
        var task = CreateTaskWithRowVersion();

        var staleVersion = Convert.ToBase64String(
            new byte[] { 9, 8, 7, 6 });

        var action = () => TaskConcurrency.EnsureCurrent(
            task,
            staleVersion);

        action.Should()
            .Throw<ConflictException>()
            .WithMessage("*modified by another request*");
    }

    private static TaskItem CreateTaskWithRowVersion()
    {
        var task = TaskItem.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test task");

        var property = typeof(TaskItem).GetProperty(
            nameof(TaskItem.RowVersion),
            BindingFlags.Instance | BindingFlags.Public);

        property.Should().NotBeNull();

        property!.SetValue(
            task,
            new byte[] { 1, 2, 3, 4 });

        return task;
    }
}
