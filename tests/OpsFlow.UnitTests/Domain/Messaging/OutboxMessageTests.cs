using FluentAssertions;
using OpsFlow.Domain.Messaging;

namespace OpsFlow.UnitTests.Domain.Messaging;

public sealed class OutboxMessageTests
{
    [Fact]
    public void CreateShouldCreateUnprocessedMessage()
    {
        var occurredAt = DateTimeOffset.UtcNow;

        var message = OutboxMessage.Create(
            "order.created",
            "{\"orderId\":\"123\"}",
            occurredAt);

        message.Id.Should().NotBeEmpty();
        message.MessageType.Should().Be("order.created");
        message.Payload.Should().Be("{\"orderId\":\"123\"}");
        message.OccurredAt.Should().Be(occurredAt);
        message.ProcessedAt.Should().BeNull();
        message.Error.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectInvalidMessageType(string messageType)
    {
        var action = () => OutboxMessage.Create(
            messageType,
            "{}",
            DateTimeOffset.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectNullMessageType()
    {
        var action = () => OutboxMessage.Create(
            null!,
            "{}",
            DateTimeOffset.UtcNow);

        action.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectInvalidPayload(string payload)
    {
        var action = () => OutboxMessage.Create(
            "order.created",
            payload,
            DateTimeOffset.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkProcessedShouldSetProcessedAtAndClearError()
    {
        var message = OutboxMessage.Create(
            "order.created",
            "{}",
            DateTimeOffset.UtcNow);

        message.MarkFailed("Temporary processing failure");

        var processedAt = DateTimeOffset.UtcNow.AddMinutes(1);

        message.MarkProcessed(processedAt);

        message.ProcessedAt.Should().Be(processedAt);
        message.Error.Should().BeNull();
    }

    [Fact]
    public void MarkFailedShouldSetErrorWithoutMarkingProcessed()
    {
        var message = OutboxMessage.Create(
            "order.created",
            "{}",
            DateTimeOffset.UtcNow);

        message.MarkFailed("Publishing failed");

        message.Error.Should().Be("Publishing failed");
        message.ProcessedAt.Should().BeNull();
    }
}
