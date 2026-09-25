using System.Text;
using System.Text.Json;
using OpsFlow.Application.Features.Tasks.Events;
using OpsFlow.Contracts.Events;
using Microsoft.Extensions.Options;
using OpsFlow.Infrastructure.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OpsFlow.Worker.Messaging;

public sealed partial class RabbitMqConsumer(
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqConsumer> logger)
{
    private readonly RabbitMqOptions _options = options.Value;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public async Task RunAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection =
            await factory.CreateConnectionAsync(stoppingToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
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

        consumer.ReceivedAsync += async (_, args) =>
        {
            try
            {
                await ProcessMessageAsync(args, stoppingToken);

                await channel.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    cancellationToken: stoppingToken);
            }
            catch (Exception exception)
            {
                LogMessageProcessingFailed(
                    args.RoutingKey,
                    exception);

                await channel.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: true,
                    cancellationToken: stoppingToken);
            }
        };

        await channel.BasicConsumeAsync(
            queue: _options.QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        LogConsumerStarted(_options.QueueName);

        await Task.Delay(
            Timeout.InfiniteTimeSpan,
            stoppingToken);
    }

    private static async Task ProcessMessageAsync(
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        var json = Encoding.UTF8.GetString(args.Body.Span);

        var envelope = JsonSerializer.Deserialize<RabbitMqEnvelope>(
            json,
            JsonOptions)
            ?? throw new InvalidOperationException(
                "RabbitMQ message envelope is invalid.");

        if (!string.Equals(
                envelope.MessageType,
                args.RoutingKey,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "RabbitMQ routing key does not match message type.");
        }

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
        EventId = 2002,
        Level = LogLevel.Error,
        Message = "RabbitMQ message processing failed for routing key {RoutingKey}")]
    private partial void LogMessageProcessingFailed(
        string routingKey,
        Exception exception);
}
