namespace OpsFlow.Worker;

public sealed partial class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            LogWorkerHeartbeat(DateTimeOffset.UtcNow);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "OpsFlow worker heartbeat at {Timestamp}")]
    private partial void LogWorkerHeartbeat(DateTimeOffset timestamp);
}
