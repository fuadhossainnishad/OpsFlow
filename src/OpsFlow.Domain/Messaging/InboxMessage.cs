namespace OpsFlow.Domain.Messaging;

public sealed class InboxMessage
{
    private InboxMessage()
    {
    }

    public Guid Id { get; private set; }
    public string MessageType { get; private set; } = null!;
    public DateTimeOffset ReceivedAt { get; private set; }
    public DateTimeOffset ProcessedAt { get; private set; }

    public static InboxMessage Create(
        Guid id,
        string messageType,
        DateTimeOffset receivedAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageType);

        return new InboxMessage
        {
            Id = id,
            MessageType = messageType,
            ReceivedAt = receivedAt,
            ProcessedAt = receivedAt
        };
    }
}
