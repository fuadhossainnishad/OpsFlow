using OpsFlow.Worker.Messaging;
using OpsFlow.Worker.Outbox;

namespace OpsFlow.Worker;

public sealed class Worker(
    RabbitMqConsumer consumer,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var consumerTask = consumer.RunAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();

            var dispatcher =
                scope.ServiceProvider.GetRequiredService<OutboxDispatcher>();

            await dispatcher.DispatchAsync(stoppingToken);

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }

        await consumerTask;
    }
}
