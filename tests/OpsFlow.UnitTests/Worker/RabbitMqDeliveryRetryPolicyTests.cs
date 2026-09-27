using FluentAssertions;
using OpsFlow.Worker.Messaging;

namespace OpsFlow.UnitTests.Worker;

public sealed class RabbitMqDeliveryRetryPolicyTests
{
    [Theory]
    [InlineData(0, 1, false)]
    [InlineData(1, 2, false)]
    [InlineData(2, 3, false)]
    [InlineData(3, 4, false)]
    [InlineData(4, 5, true)]
    public void AttemptsShouldProgressToDeadLetter(
        int currentAttempt,
        int expectedNextAttempt,
        bool expectedDeadLetter)
    {
        RabbitMqDeliveryRetryPolicy.NextAttempt(currentAttempt)
            .Should().Be(expectedNextAttempt);

        RabbitMqDeliveryRetryPolicy.ShouldDeadLetter(currentAttempt)
            .Should().Be(expectedDeadLetter);
    }
}
