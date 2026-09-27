using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Contracts.Events;
using OpsFlow.Infrastructure.Persistence;
using OpsFlow.Worker;

namespace OpsFlow.Worker.Outbox;

public sealed partial class OutboxDispatcher(
    OpsFlowDbContext dbContext,
    IMessagePublisher messagePublisher,
    ILogger<OutboxDispatcher> logger) : IOutboxDispatcher
{
    private const int BatchSize = 20;
    private const int MaxDeliveryAttempts = 8;
    private static readonly TimeSpan InitialRetryDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan MaximumRetryDelay = TimeSpan.FromMinutes(5);

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task DispatchAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database
            .BeginTransactionAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var messages = await dbContext.OutboxMessages
            .FromSqlInterpolated($"""
                SELECT TOP ({BatchSize}) *
                FROM [OutboxMessages] WITH (UPDLOCK, READPAST, ROWLOCK)
                WHERE [ProcessedAt] IS NULL
                  AND [DeadLetteredAt] IS NULL
                  AND ([NextAttemptAt] IS NULL OR [NextAttemptAt] <= {now})
                ORDER BY [OccurredAt]
                """)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await PublishAsync(message, cancellationToken);

                message.MarkProcessed(DateTimeOffset.UtcNow);

                await dbContext.SaveChangesAsync(cancellationToken);

                LogMessageProcessed(message.Id, message.MessageType);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                var failedAt = DateTimeOffset.UtcNow;
                var delay = GetRetryDelay(message.DeliveryAttempts + 1);
                message.MarkFailed(
                    exception.Message,
                    failedAt,
                    delay,
                    MaxDeliveryAttempts);

                await dbContext.SaveChangesAsync(cancellationToken);

                if (message.DeadLetteredAt.HasValue)
                {
                    LogMessageDeadLettered(
                        message.Id,
                        message.MessageType,
                        message.DeliveryAttempts,
                        exception);
                }
                else
                {
                    LogMessageFailed(
                        message.Id,
                        message.MessageType,
                        message.DeliveryAttempts,
                        message.NextAttemptAt!.Value,
                        exception);
                }
            }
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private static TimeSpan GetRetryDelay(int attempt)
    {
        var multiplier = Math.Pow(2, Math.Clamp(attempt - 1, 0, 20));
        var seconds = Math.Min(
            InitialRetryDelay.TotalSeconds * multiplier,
            MaximumRetryDelay.TotalSeconds);
        return TimeSpan.FromSeconds(seconds);
    }

    private async Task PublishAsync(
        OpsFlow.Domain.Messaging.OutboxMessage message,
        CancellationToken cancellationToken)
    {
        switch (message.MessageType)
        {
            case "task.created":
                var taskCreated = JsonSerializer.Deserialize<TaskCreatedEvent>(
                    message.Payload,
                    JsonOptions);

                if (taskCreated is null)
                {
                    throw new InvalidOperationException(
                        $"Invalid payload for outbox message '{message.Id}'.");
                }

                await messagePublisher.PublishAsync(
                    message.Id,
                    message.MessageType,
                    taskCreated,
                    cancellationToken);
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported outbox message type '{message.MessageType}'.");
        }
    }

    [LoggerMessage(
        EventId = 2100,
        Level = LogLevel.Information,
        Message = "Outbox message {MessageId} processed: {MessageType}")]
    private partial void LogMessageProcessed(
        Guid messageId,
        string messageType);

    [LoggerMessage(
        EventId = 2101,
        Level = LogLevel.Error,
        Message = "Outbox message {MessageId} attempt {Attempt} failed: {MessageType}; retry after {NextAttemptAt}")]
    private partial void LogMessageFailed(
        Guid messageId,
        string messageType,
        int attempt,
        DateTimeOffset nextAttemptAt,
        Exception exception);

    [LoggerMessage(
        EventId = 2102,
        Level = LogLevel.Critical,
        Message = "Outbox message {MessageId} dead-lettered after {Attempt} attempts: {MessageType}")]
    private partial void LogMessageDeadLettered(
        Guid messageId,
        string messageType,
        int attempt,
        Exception exception);
}
