using FluentAssertions;
using OpsFlow.Domain.Messaging;

namespace OpsFlow.UnitTests.Domain.Messaging;

public sealed class InboxMessageTests
{
    [Fact]
    public void CreateShouldInitializeMessage()
    {
        var id = Guid.NewGuid();
        var receivedAt = DateTimeOffset.UtcNow;

        var message = InboxMessage.Create(
            id,
            "task.created",
            receivedAt);

        message.Id.Should().Be(id);
        message.MessageType.Should().Be("task.created");
        message.ReceivedAt.Should().Be(receivedAt);
        message.ProcessedAt.Should().Be(receivedAt);
    }

    [Fact]
    public void CreateShouldRejectEmptyMessageType()
    {
        var action = () => InboxMessage.Create(
            Guid.NewGuid(),
            "   ",
            DateTimeOffset.UtcNow);

        action.Should()
            .Throw<ArgumentException>();
    }
}
