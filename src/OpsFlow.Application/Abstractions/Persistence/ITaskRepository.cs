using OpsFlow.Domain.Tasks;

namespace OpsFlow.Application.Abstractions.Persistence;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(
        Guid organizationId,
        Guid taskId,
        CancellationToken cancellationToken);

    Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken);
}
