using OpsFlow.Domain.TimeTracking;

namespace OpsFlow.Application.Features.TimeTracking;

public interface ITimeEntryRepository
{
    Task AddAsync(TimeEntry timeEntry, CancellationToken cancellationToken);

    Task<TimeEntry?> GetByIdAsync(
        Guid organizationId,
        Guid userId,
        Guid timeEntryId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TimeEntryRecord>> GetAsync(
        Guid organizationId,
        Guid userId,
        Guid? projectId,
        Guid? taskId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        CancellationToken cancellationToken);

    Task<bool> HasRunningEntryAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken);

    void Remove(TimeEntry timeEntry);
}
