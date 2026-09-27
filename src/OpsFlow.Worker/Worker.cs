using OpsFlow.Worker.Messaging;
using OpsFlow.Worker.Outbox;

namespace OpsFlow.Worker;

public interface IRabbitMqConsumer
{
    Task RunAsync(CancellationToken stoppingToken);
}

public interface IOutboxDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken);
}

public sealed partial class Worker(
    IServiceScopeFactory scopeFactory,
    ILogger<Worker> logger) : BackgroundService
{
    private const int MaximumRetryDelaySeconds = 60;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var consumerTask = SuperviseConsumerAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunDispatcherAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                LogOutboxDispatchFailed(exception);
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        await consumerTask;
    }

    private async Task SuperviseConsumerAsync(
        CancellationToken cancellationToken)
    {
        var attempt = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var consumer = scope.ServiceProvider
                    .GetRequiredService<IRabbitMqConsumer>();

                await consumer.RunAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    return;

                attempt++;
                LogConsumerStopped();
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                attempt++;
                LogConsumerFailed(exception);
            }

            var delaySeconds = Math.Min(
                Math.Pow(2, Math.Clamp(attempt - 1, 0, 20)),
                MaximumRetryDelaySeconds);

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(delaySeconds),
                    cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    private async Task RunDispatcherAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            scopeFactory.CreateAsyncScope();

        var dispatcher =
            scope.ServiceProvider.GetRequiredService<IOutboxDispatcher>();

        await dispatcher.DispatchAsync(cancellationToken);
    }

    [LoggerMessage(
        EventId = 3000,
        Level = LogLevel.Error,
        Message = "Outbox dispatch failed; it will be retried.")]
    private partial void LogOutboxDispatchFailed(Exception exception);

    [LoggerMessage(
        EventId = 3001,
        Level = LogLevel.Warning,
        Message = "RabbitMQ consumer stopped unexpectedly; restarting.")]
    private partial void LogConsumerStopped();

    [LoggerMessage(
        EventId = 3002,
        Level = LogLevel.Error,
        Message = "RabbitMQ consumer failed; restarting with backoff.")]
    private partial void LogConsumerFailed(Exception exception);
}
