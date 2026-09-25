namespace OpsFlow.Application.Abstractions.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(
        string messageType,
        TMessage message,
        CancellationToken cancellationToken);
}
