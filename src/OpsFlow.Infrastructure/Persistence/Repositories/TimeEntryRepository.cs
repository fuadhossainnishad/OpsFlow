using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Features.TimeTracking;
using OpsFlow.Domain.TimeTracking;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class TimeEntryRepository(
    OpsFlowDbContext dbContext) : ITimeEntryRepository
{
    public async Task AddAsync(
        TimeEntry timeEntry,
        CancellationToken cancellationToken)
        => await dbContext.TimeEntries.AddAsync(
            timeEntry,
            cancellationToken);

    public Task<TimeEntry?> GetByIdAsync(
        Guid organizationId,
        Guid userId,
        Guid timeEntryId,
        CancellationToken cancellationToken)
        => dbContext.TimeEntries.SingleOrDefaultAsync(
            entry =>
                entry.Id == timeEntryId &&
                entry.OrganizationId == organizationId &&
                entry.UserId == userId,
            cancellationToken);

    public async Task<IReadOnlyList<TimeEntryRecord>> GetAsync(
        Guid organizationId,
        Guid userId,
        Guid? projectId,
        Guid? taskId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        CancellationToken cancellationToken)
        => await dbContext.TimeEntries
            .AsNoTracking()
            .Where(entry =>
                entry.OrganizationId == organizationId &&
                entry.UserId == userId &&
                (!projectId.HasValue || entry.ProjectId == projectId.Value) &&
                (!taskId.HasValue || entry.TaskId == taskId.Value) &&
                (!fromUtc.HasValue || entry.StartedAtUtc >= fromUtc.Value) &&
                (!toUtc.HasValue || entry.StartedAtUtc < toUtc.Value))
            .OrderByDescending(entry => entry.StartedAtUtc)
            .Select(entry => new TimeEntryRecord(
                entry.Id,
                entry.OrganizationId,
                entry.UserId,
                entry.ProjectId,
                entry.TaskId,
                entry.Description,
                entry.StartedAtUtc,
                entry.EndedAtUtc,
                entry.DurationSeconds,
                entry.IsManual,
                entry.CreatedAtUtc,
                entry.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

    public Task<bool> HasRunningEntryAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken)
        => dbContext.TimeEntries.AnyAsync(
            entry =>
                entry.OrganizationId == organizationId &&
                entry.UserId == userId &&
                entry.EndedAtUtc == null,
            cancellationToken);

    public void Remove(TimeEntry timeEntry)
        => dbContext.TimeEntries.Remove(timeEntry);
}
