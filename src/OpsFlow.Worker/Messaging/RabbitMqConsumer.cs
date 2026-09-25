using System.Text;
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

    public async Task RunAsync(
        CancellationToken stoppingToken)
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
                var message = Encoding.UTF8.GetString(args.Body.Span);

                LogMessageReceived(args.RoutingKey);

                // Message handlers will be registered here as the
                // individual asynchronous workflows are introduced.
                _ = message;

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

    [LoggerMessage(
        EventId = 2000,
        Level = LogLevel.Information,
        Message = "RabbitMQ consumer started for queue {QueueName}")]
    private partial void LogConsumerStarted(string queueName);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Debug,
        Message = "RabbitMQ message received with routing key {RoutingKey}")]
    private partial void LogMessageReceived(string routingKey);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Error,
        Message = "RabbitMQ message processing failed for routing key {RoutingKey}")]
    private partial void LogMessageProcessingFailed(
        string routingKey,
        Exception exception);
}
