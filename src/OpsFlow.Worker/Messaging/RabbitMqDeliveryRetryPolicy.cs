namespace OpsFlow.Worker.Messaging;

public static class RabbitMqDeliveryRetryPolicy
{
    public const int MaximumAttempts = 5;

    public static int NextAttempt(int currentAttempt) => currentAttempt + 1;

    public static bool ShouldDeadLetter(int currentAttempt) =>
        NextAttempt(currentAttempt) >= MaximumAttempts;
}
