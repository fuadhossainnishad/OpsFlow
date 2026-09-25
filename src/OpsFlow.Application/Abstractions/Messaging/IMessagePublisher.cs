namespace OpsFlow.Application.Abstractions.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(
        Guid messageId,
        string messageType,
        TMessage message,
        CancellationToken cancellationToken);
}
