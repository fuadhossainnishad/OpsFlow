namespace OpsFlow.Contracts.Events;

public sealed record TaskCreatedEvent(
    Guid TaskId,
    Guid OrganizationId,
    Guid ProjectId,
    string Title,
    DateTimeOffset OccurredAt);
