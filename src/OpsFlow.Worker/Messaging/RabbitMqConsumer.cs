using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Features.Tasks.Events;
using OpsFlow.Contracts.Events;
using OpsFlow.Domain.Messaging;
using OpsFlow.Infrastructure.Messaging;
using OpsFlow.Infrastructure.Persistence;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Options;
using OpsFlow.Application.Abstractions.Notifications;
using OpsFlow.Worker;

namespace OpsFlow.Worker.Messaging;

public sealed partial class RabbitMqConsumer(
    IOptions<RabbitMqOptions> options,
    OpsFlowDbContext dbContext,
    INotificationRepository notificationRepository,
    ILogger<RabbitMqConsumer> logger) : IRabbitMqConsumer
{
    private readonly RabbitMqOptions _options = options.Value;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private const string RetryHeader = "x-opsflow-retry-attempt";
    private const string DeadLetterRoutingKey = "dead-letter";
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true
        };

        await using var connection =
            await factory.CreateConnectionAsync(stoppingToken);

        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true),
            stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        var deadLetterExchange = $"{_options.ExchangeName}.dead-letter";
        var deadLetterQueue = $"{_options.QueueName}.dead-letter";
        var retryExchange = $"{_options.ExchangeName}.retry";
        var retryQueue = $"{_options.QueueName}.retry";

        await channel.ExchangeDeclareAsync(
            exchange: deadLetterExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: deadLetterQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: deadLetterQueue,
            exchange: deadLetterExchange,
            routingKey: "#",
            cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: retryExchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: retryQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>
            {
                ["x-message-ttl"] = (int)RetryDelay.TotalMilliseconds,
                ["x-dead-letter-exchange"] = _options.ExchangeName
            },
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: retryQueue,
            exchange: retryExchange,
            routingKey: "#",
            cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: stoppingToken);

        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: "#",
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        using var deliveryLock = new SemaphoreSlim(1, 1);

        consumer.ReceivedAsync += async (_, args) =>
        {
            var lockTaken = false;
            try
            {
                await deliveryLock.WaitAsync(stoppingToken);
                lockTaken = true;
                await ProcessMessageAsync(args, stoppingToken);

                await channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Leave in-flight deliveries unacknowledged; closing the channel redelivers them.
            }
            catch (Exception exception) when (
                exception is not OperationCanceledException)
            {
                LogMessageProcessingFailed(
                    args.RoutingKey,
                    exception);

                try
                {
                    await RouteFailedDeliveryAsync(
                        channel,
                        args,
                        exception,
                        stoppingToken);

                    await channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);
                }
                catch (Exception republishException) when (
                    republishException is not OperationCanceledException)
                {
                    LogFailedDeliveryRouting(args.RoutingKey, republishException);
                    await channel.BasicNackAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: stoppingToken);
                }
            }
            finally
            {
                if (lockTaken)
                    deliveryLock.Release();
            }
        };

        var consumerTag = await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        LogConsumerStarted(_options.QueueName);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            if (channel.IsOpen)
            {
                await channel.BasicCancelAsync(
                    consumerTag,
                    noWait: false,
                    cancellationToken: CancellationToken.None);
            }

            await deliveryLock.WaitAsync(CancellationToken.None);
            deliveryLock.Release();
        }
    }

    private async Task RouteFailedDeliveryAsync(
        IChannel channel,
        BasicDeliverEventArgs args,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var attempts = GetRetryAttempt(args.BasicProperties.Headers);
        var properties = CopyProperties(args.BasicProperties);

        if (RabbitMqDeliveryRetryPolicy.ShouldDeadLetter(attempts))
        {
            await channel.BasicPublishAsync(
                exchange: $"{_options.ExchangeName}.dead-letter",
                routingKey: DeadLetterRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: args.Body,
                cancellationToken: cancellationToken);

            LogDeliveryDeadLettered(args.RoutingKey, attempts + 1, exception);
            return;
        }

        properties.Headers ??= new Dictionary<string, object?>();
        properties.Headers[RetryHeader] =
            RabbitMqDeliveryRetryPolicy.NextAttempt(attempts);
        properties.Expiration = ((int)RetryDelay.TotalMilliseconds)
            .ToString(System.Globalization.CultureInfo.InvariantCulture);

        await channel.BasicPublishAsync(
            exchange: $"{_options.ExchangeName}.retry",
            routingKey: args.RoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: args.Body,
            cancellationToken: cancellationToken);
    }

    private static int GetRetryAttempt(IDictionary<string, object?>? headers)
    {
        if (headers is null || !headers.TryGetValue(RetryHeader, out var value))
            return 0;

        return value switch
        {
            byte attempt => attempt,
            short attempt => attempt,
            int attempt => attempt,
            long attempt when attempt <= int.MaxValue => (int)attempt,
            byte[] bytes when int.TryParse(
                Encoding.UTF8.GetString(bytes),
                out var attempt) => attempt,
            _ => 0
        };
    }

    private static BasicProperties CopyProperties(IReadOnlyBasicProperties source)
    {
        return new BasicProperties
        {
            ContentType = source.ContentType,
            ContentEncoding = source.ContentEncoding,
            DeliveryMode = source.DeliveryMode,
            MessageId = source.MessageId,
            Type = source.Type,
            Timestamp = source.Timestamp,
            Headers = source.Headers is null
                ? new Dictionary<string, object?>()
                : new Dictionary<string, object?>(source.Headers)
        };
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        var json = Encoding.UTF8.GetString(args.Body.Span);

        var envelope = JsonSerializer.Deserialize<RabbitMqEnvelope>(
            json,
            JsonOptions)
            ?? throw new InvalidOperationException(
                "RabbitMQ message envelope is invalid.");

        if (envelope.MessageId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "RabbitMQ message ID is missing.");
        }

        if (!string.Equals(
                envelope.MessageType,
                args.RoutingKey,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "RabbitMQ routing key does not match message type.");
        }

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var alreadyProcessed = await dbContext.InboxMessages
            .AsNoTracking()
            .AnyAsync(
                message => message.Id == envelope.MessageId,
                cancellationToken);

        if (alreadyProcessed)
        {
            await transaction.CommitAsync(cancellationToken);

            LogDuplicateMessage(
                envelope.MessageId,
                envelope.MessageType);

            return;
        }

        await ProcessPayloadAsync(
            envelope,
            cancellationToken);

        dbContext.InboxMessages.Add(
            InboxMessage.Create(
                envelope.MessageId,
                envelope.MessageType,
                DateTimeOffset.UtcNow));

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private  async Task ProcessPayloadAsync(
        RabbitMqEnvelope envelope,
        CancellationToken cancellationToken)
    {
        switch (envelope.MessageType)
        {
            case "task.created":
                {
                    var message = envelope.Payload.Deserialize<TaskCreatedEvent>(
                        JsonOptions)
                        ?? throw new InvalidOperationException(
                            "TaskCreatedEvent payload is invalid.");

                    await TaskCreatedProcessor.ProcessAsync(
                        message,
                            notificationRepository,
                        cancellationToken);

                    break;
                }

            default:
                throw new InvalidOperationException(
                    $"Unsupported message type '{envelope.MessageType}'.");
        }
    }

    private sealed record RabbitMqEnvelope(
        Guid MessageId,
        string MessageType,
        DateTimeOffset OccurredAt,
        JsonElement Payload);

    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "RabbitMQ consumer started for queue {QueueName}")]
    private partial void LogConsumerStarted(string queueName);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Debug,
        Message = "RabbitMQ duplicate message ignored: {MessageId} ({MessageType})")]
    private partial void LogDuplicateMessage(
        Guid messageId,
        string messageType);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Error,
        Message = "RabbitMQ message processing failed for routing key {RoutingKey}")]
    private partial void LogMessageProcessingFailed(
        string routingKey,
        Exception exception);

    [LoggerMessage(
        EventId = 2003,
        Level = LogLevel.Warning,
        Message = "RabbitMQ message dead-lettered after {Attempt} attempts: {RoutingKey}")]
    private partial void LogDeliveryDeadLettered(
        string routingKey,
        int attempt,
        Exception exception);

    [LoggerMessage(
        EventId = 2004,
        Level = LogLevel.Error,
        Message = "Failed to route RabbitMQ delivery for {RoutingKey}; original delivery will be requeued")]
    private partial void LogFailedDeliveryRouting(
        string routingKey,
        Exception exception);
}
