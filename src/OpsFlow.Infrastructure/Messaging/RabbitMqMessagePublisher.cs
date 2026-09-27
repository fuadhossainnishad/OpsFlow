using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OpsFlow.Application.Abstractions.Messaging;
using RabbitMQ.Client;

namespace OpsFlow.Infrastructure.Messaging;

public sealed class RabbitMqMessagePublisher(
    IOptions<RabbitMqOptions> options) : IMessagePublisher, IAsyncDisposable
{
    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly SemaphoreSlim _publishLock = new(1, 1);

    public async Task PublishAsync<TMessage>(
        Guid messageId,
        string messageType,
        TMessage message,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);
        ArgumentNullException.ThrowIfNull(message);

        await _publishLock.WaitAsync(cancellationToken);
        try
        {
            await EnsureInitializedAsync(cancellationToken);

            var envelope = new
            {
                MessageId = messageId,
                MessageType = messageType,
                OccurredAt = DateTimeOffset.UtcNow,
                Payload = message
            };

            var body = JsonSerializer.SerializeToUtf8Bytes(envelope);

            var properties = new BasicProperties
            {
                ContentType = "application/json",
                ContentEncoding = "utf-8",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = messageId.ToString(),
                Type = messageType,
                Timestamp = new AmqpTimestamp(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _channel!.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: messageType,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await ResetConnectionAsync();
            throw;
        }
        finally
        {
            _publishLock.Release();
        }
    }

    private async Task EnsureInitializedAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
        {
            return;
        }

        await ResetConnectionAsync();

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true
        };

        IConnection? connection = null;
        IChannel? channel = null;
        try
        {
            connection = await factory.CreateConnectionAsync(cancellationToken);

            channel = await connection.CreateChannelAsync(
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true),
                cancellationToken);

            await channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken);

            _connection = connection;
            _channel = channel;
        }
        catch
        {
            if (channel is not null)
            {
                await channel.DisposeAsync();
            }

            if (connection is not null)
            {
                await connection.DisposeAsync();
            }

            throw;
        }
    }

    private async Task ResetConnectionAsync()
    {
        var channel = _channel;
        var connection = _connection;
        _channel = null;
        _connection = null;

        if (channel is not null)
        {
            try { await channel.DisposeAsync(); }
            catch { }
        }

        if (connection is not null)
        {
            try { await connection.DisposeAsync(); }
            catch { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _publishLock.WaitAsync();
        try { await ResetConnectionAsync(); }
        finally
        {
            _publishLock.Release();
            _publishLock.Dispose();
        }
    }
}
