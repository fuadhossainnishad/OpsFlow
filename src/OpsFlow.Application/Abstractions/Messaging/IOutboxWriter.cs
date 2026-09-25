namespace OpsFlow.Application.Abstractions.Messaging;

public interface IOutboxWriter
{
    Task AddAsync(
        string messageType,
        object message,
        CancellationToken cancellationToken);
}
