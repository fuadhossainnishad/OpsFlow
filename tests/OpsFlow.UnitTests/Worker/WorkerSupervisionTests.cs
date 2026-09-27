using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpsFlow.Worker;

namespace OpsFlow.UnitTests.Worker;

public sealed class WorkerSupervisionTests
{
    [Fact]
    public async Task ConsumerIsRestartedAfterUnexpectedFailureAndStopsOnCancellation()
    {
        var consumer = new FailOnceConsumer();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IRabbitMqConsumer>(_ => consumer);
        services.AddScoped<IOutboxDispatcher, NoOpOutboxDispatcher>();

        await using var provider = services.BuildServiceProvider();
        var worker = new global::OpsFlow.Worker.Worker(
            provider.GetRequiredService<IServiceScopeFactory>(),
            provider.GetRequiredService<ILogger<global::OpsFlow.Worker.Worker>>());

        await worker.StartAsync(CancellationToken.None);
        await consumer.Restarted.Task.WaitAsync(TimeSpan.FromSeconds(5));

        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await worker.StopAsync(timeout.Token);

        consumer.Calls.Should().BeGreaterThanOrEqualTo(2);
    }

    private sealed class FailOnceConsumer : IRabbitMqConsumer
    {
        private int _calls;

        public TaskCompletionSource Restarted { get; } = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        public int Calls => Volatile.Read(ref _calls);

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _calls) == 1)
                throw new InvalidOperationException("Simulated broker failure.");

            Restarted.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
    }

    private sealed class NoOpOutboxDispatcher : IOutboxDispatcher
    {
        public Task DispatchAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}
