using System.Text.Json;
using OpsFlow.Application.Abstractions.Messaging;
using OpsFlow.Domain.Messaging;
using OpsFlow.Infrastructure.Persistence;

namespace OpsFlow.Infrastructure.Messaging;

public sealed class OutboxWriter(
    OpsFlowDbContext dbContext) : IOutboxWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(
        JsonSerializerDefaults.Web);

    public Task AddAsync(
        string messageType,
        object message,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentNullException.ThrowIfNull(message);

        var payload = JsonSerializer.Serialize(message, JsonOptions);

        var outboxMessage = OutboxMessage.Create(
            messageType,
            payload,
            DateTimeOffset.UtcNow);

        dbContext.OutboxMessages.Add(outboxMessage);

        return Task.CompletedTask;
    }
}
