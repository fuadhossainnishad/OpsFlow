using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Reports;
using OpsFlow.Application.Features.Reports.GetDashboardSummary;
using OpsFlow.Application.Features.Reports.GetProjectReport;
using OpsFlow.Application.Features.Reports.GetTimeReport;
using OpsFlow.Domain.Projects;
using OpsFlow.Domain.Tasks;
using DomainTaskStatus = OpsFlow.Domain.Tasks.TaskStatus;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class ReportRepository(
    OpsFlowDbContext dbContext) : IReportRepository
{
    public async Task<DashboardSummaryResult> GetDashboardAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var totalProjects = await dbContext.Projects
            .CountAsync(x => x.OrganizationId == organizationId, cancellationToken);

        var activeProjects = await dbContext.Projects
            .CountAsync(
                x => x.OrganizationId == organizationId &&
                     x.Status == ProjectStatus.Active,
                cancellationToken);

        var taskCounts = await dbContext.Tasks
            .Where(x => x.OrganizationId == organizationId)
            .GroupBy(x => x.Status)
            .Select(x => new
            {
                Status = x.Key,
                Count = x.Count()
            })
            .ToListAsync(cancellationToken);

        var totalTasks = taskCounts.Sum(x => x.Count);
        var todoTasks = taskCounts
            .Where(x => x.Status == DomainTaskStatus.Todo)
            .Select(x => x.Count)
            .FirstOrDefault();
        var inProgressTasks = taskCounts
            .Where(x => x.Status == DomainTaskStatus.InProgress)
            .Select(x => x.Count)
            .FirstOrDefault();
        var doneTasks = taskCounts
            .Where(x => x.Status == DomainTaskStatus.Done)
            .Select(x => x.Count)
            .FirstOrDefault();
        var cancelledTasks = taskCounts
            .Where(x => x.Status == DomainTaskStatus.Cancelled)
            .Select(x => x.Count)
            .FirstOrDefault();

        var pendingApprovals = await dbContext.ApprovalRequests
            .CountAsync(
                x => x.OrganizationId == organizationId &&
                     x.Status == Domain.Approvals.ApprovalStatus.Pending,
                cancellationToken);

        var runningTimers = await dbContext.TimeEntries
            .CountAsync(
                x => x.OrganizationId == organizationId &&
                     x.EndedAtUtc == null,
                cancellationToken);

        var trackedSeconds = await dbContext.TimeEntries
            .Where(x => x.OrganizationId == organizationId)
            .Select(x => x.DurationSeconds ?? 0L)
            .SumAsync(cancellationToken);

        return new DashboardSummaryResult(
            totalProjects,
            activeProjects,
            totalTasks,
            todoTasks,
            inProgressTasks,
            doneTasks,
            cancelledTasks,
            pendingApprovals,
            runningTimers,
            trackedSeconds);
    }

    public async Task<IReadOnlyList<TimeReportRow>> GetTimeReportAsync(
        Guid organizationId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken)
    {
        return await (
            from entry in dbContext.TimeEntries.AsNoTracking()
            join project in dbContext.Projects.AsNoTracking()
                on entry.ProjectId equals project.Id
            where entry.OrganizationId == organizationId
                  && project.OrganizationId == organizationId
                  && entry.StartedAtUtc >= fromUtc
                  && entry.StartedAtUtc < toUtc
                  && entry.EndedAtUtc != null
            group entry by new
            {
                project.Id,
                project.Name
            }
            into grouped
            orderby grouped.Sum(x => x.DurationSeconds ?? 0L) descending
            select new TimeReportRow(
                grouped.Key.Id,
                grouped.Key.Name,
                grouped.Sum(x => x.DurationSeconds ?? 0L),
                grouped.Count()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectReportRow>> GetProjectReportAsync(
        Guid organizationId,
        CancellationToken cancellationToken)
    {
        var projects = await dbContext.Projects
            .AsNoTracking()
            .Where(x => x.OrganizationId == organizationId)
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Key,
                x.Name,
                x.Status
            })
            .ToListAsync(cancellationToken);

        var taskStats = await dbContext.Tasks
            .AsNoTracking()
            .Where(x => x.OrganizationId == organizationId)
            .GroupBy(x => x.ProjectId)
            .Select(x => new
            {
                ProjectId = x.Key,
                Total = x.Count(),
                Completed = x.Count(t => t.Status == DomainTaskStatus.Done)
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        var timeStats = await dbContext.TimeEntries
            .AsNoTracking()
            .Where(x => x.OrganizationId == organizationId)
            .GroupBy(x => x.ProjectId)
            .Select(x => new
            {
                ProjectId = x.Key,
                Seconds = x.Sum(t => t.DurationSeconds ?? 0L),
                EntryCount = x.Count()
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        return projects
            .Select(project =>
            {
                taskStats.TryGetValue(project.Id, out var tasks);
                timeStats.TryGetValue(project.Id, out var time);

                return new ProjectReportRow(
                    project.Id,
                    project.Key,
                    project.Name,
                    project.Status.ToString(),
                    tasks?.Total ?? 0,
                    tasks?.Completed ?? 0,
                    (tasks?.Total ?? 0) - (tasks?.Completed ?? 0),
                    time?.Seconds ?? 0L,
                    time?.EntryCount ?? 0);
            })
            .ToList();
    }
}
