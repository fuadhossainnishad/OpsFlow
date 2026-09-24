using Microsoft.EntityFrameworkCore;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(
    OpsFlowDbContext dbContext) : ITaskRepository
{
    public async Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken)
        => await dbContext.Tasks.AddAsync(
            task,
            cancellationToken);

    public Task<TaskItem?> GetByIdAsync(
    Guid organizationId,
    Guid taskId,
    CancellationToken cancellationToken)
    => dbContext.Tasks.SingleOrDefaultAsync(
        task =>
            task.Id == taskId &&
            task.OrganizationId == organizationId,
        cancellationToken);
}
