using OpsFlow.Worker.Messaging;
using OpsFlow.Worker.Outbox;

namespace OpsFlow.Worker;

public sealed class Worker(
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();

        var consumer =
            scope.ServiceProvider.GetRequiredService<RabbitMqConsumer>();

        var dispatcher =
            scope.ServiceProvider.GetRequiredService<OutboxDispatcher>();

        var consumerTask = consumer.RunAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await dispatcher.DispatchAsync(stoppingToken);

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }

        await consumerTask;
    }
}
