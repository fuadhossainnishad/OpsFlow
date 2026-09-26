using OpsFlow.Worker.Messaging;
using OpsFlow.Worker.Outbox;

namespace OpsFlow.Worker;

public sealed class Worker(
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var consumerTask = RunConsumerAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunDispatcherAsync(stoppingToken);

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }

        await consumerTask;
    }

    private async Task RunConsumerAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            scopeFactory.CreateAsyncScope();

        var consumer =
            scope.ServiceProvider.GetRequiredService<RabbitMqConsumer>();

        await consumer.RunAsync(cancellationToken);
    }

    private async Task RunDispatcherAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            scopeFactory.CreateAsyncScope();

        var dispatcher =
            scope.ServiceProvider.GetRequiredService<OutboxDispatcher>();

        await dispatcher.DispatchAsync(cancellationToken);
    }
}
