using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Contracts.Events;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.Worker.Outbox;

public sealed partial class OutboxDispatcher(
    OpsFlowDbContext dbContext,
    IMessagePublisher messagePublisher,
    ILogger<OutboxDispatcher> logger)
{
    private const int BatchSize = 20;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task DispatchAsync(CancellationToken cancellationToken)
    {
        var messages = await dbContext.OutboxMessages
            .Where(message => message.ProcessedAt == null)
            .OrderBy(message => message.OccurredAt)
            .Take(BatchSize)
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
                message.MarkFailed(exception.Message);

                await dbContext.SaveChangesAsync(cancellationToken);

                LogMessageFailed(
                    message.Id,
                    message.MessageType,
                    exception);
            }
        }
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
        Message = "Outbox message {MessageId} failed: {MessageType}")]
    private partial void LogMessageFailed(
        Guid messageId,
        string messageType,
        Exception exception);
}
