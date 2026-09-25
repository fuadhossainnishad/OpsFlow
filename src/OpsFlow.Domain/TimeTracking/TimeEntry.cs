using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.TimeTracking;

public sealed class TimeEntry : Entity
{
    private TimeEntry() { }

    private TimeEntry(
        Guid id,
        Guid organizationId,
        Guid userId,
        Guid projectId,
        Guid? taskId,
        string? description,
        DateTimeOffset startedAtUtc,
        DateTimeOffset? endedAtUtc,
        bool isManual)
    {
        Id = id;
        OrganizationId = organizationId;
        UserId = userId;
        ProjectId = projectId;
        TaskId = taskId;
        Description = description;
        StartedAtUtc = startedAtUtc;
        EndedAtUtc = endedAtUtc;
        IsManual = isManual;
        CreatedAtUtc = DateTimeOffset.UtcNow;
        DurationSeconds = endedAtUtc.HasValue
            ? CalculateDuration(startedAtUtc, endedAtUtc.Value)
            : null;
    }

    public Guid OrganizationId { get; private init; }
    public Guid UserId { get; private init; }
    public Guid ProjectId { get; private init; }
    public Guid? TaskId { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset? EndedAtUtc { get; private set; }
    public long? DurationSeconds { get; private set; }
    public bool IsManual { get; private init; }
    public DateTimeOffset CreatedAtUtc { get; private init; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public bool IsRunning => EndedAtUtc is null;

    public static TimeEntry Start(
        Guid organizationId,
        Guid userId,
        Guid projectId,
        Guid? taskId,
        string? description)
    {
        ValidateIds(organizationId, userId, projectId);
        return new TimeEntry(
            Guid.NewGuid(),
            organizationId,
            userId,
            projectId,
            taskId,
            NormalizeDescription(description),
            DateTimeOffset.UtcNow,
            null,
            false);
    }

    public static TimeEntry CreateManual(
        Guid organizationId,
        Guid userId,
        Guid projectId,
        Guid? taskId,
        string? description,
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc)
    {
        ValidateIds(organizationId, userId, projectId);

        if (endedAtUtc <= startedAtUtc)
        {
            throw new ArgumentException(
                "End time must be after start time.",
                nameof(endedAtUtc));
        }

        return new TimeEntry(
            Guid.NewGuid(),
            organizationId,
            userId,
            projectId,
            taskId,
            NormalizeDescription(description),
            startedAtUtc,
            endedAtUtc,
            true);
    }

    public void Stop(DateTimeOffset endedAtUtc)
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException(
                "The time entry has already been stopped.");
        }

        if (endedAtUtc <= StartedAtUtc)
        {
            throw new ArgumentException(
                "End time must be after start time.",
                nameof(endedAtUtc));
        }

        EndedAtUtc = endedAtUtc;
        DurationSeconds = CalculateDuration(StartedAtUtc, endedAtUtc);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    public void Update(
        Guid? taskId,
        string? description,
        DateTimeOffset startedAtUtc,
        DateTimeOffset? endedAtUtc)
    {
        if (endedAtUtc.HasValue && endedAtUtc.Value <= startedAtUtc)
        {
            throw new ArgumentException(
                "End time must be after start time.",
                nameof(endedAtUtc));
        }

        StartedAtUtc = startedAtUtc;
        EndedAtUtc = endedAtUtc;
        DurationSeconds = endedAtUtc.HasValue
            ? CalculateDuration(startedAtUtc, endedAtUtc.Value)
            : null;
        TaskId = taskId;
        Description = NormalizeDescription(description);
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }

    private static void ValidateIds(
        Guid organizationId,
        Guid userId,
        Guid projectId)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required.", nameof(organizationId));

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (projectId == Guid.Empty)
            throw new ArgumentException("Project ID is required.", nameof(projectId));
    }

    private static string? NormalizeDescription(string? description)
        => string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

    private static long CalculateDuration(
        DateTimeOffset startedAtUtc,
        DateTimeOffset endedAtUtc)
        => checked((long)(endedAtUtc - startedAtUtc).TotalSeconds);
}
