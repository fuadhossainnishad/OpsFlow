using OpsFlow.Contracts.Events;

namespace OpsFlow.Application.Features.Tasks.Events;

public static class TaskCreatedProcessor
{
    public static Task ProcessAsync(
        TaskCreatedEvent message,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        return Task.CompletedTask;
    }
}
