using OpsFlow.Worker.Messaging;

namespace OpsFlow.Worker;

public sealed class Worker(
    RabbitMqConsumer consumer) : BackgroundService
{
    protected override Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        return consumer.RunAsync(stoppingToken);
    }
}
