using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Tasks;

public sealed class TaskItem : Entity
{
    private TaskItem() { }

    private TaskItem(
        Guid id,
        Guid organizationId,
        Guid projectId,
        string title,
        string? description,
        Guid? assigneeUserId)
    {
        Id = id;
        OrganizationId = organizationId;
        ProjectId = projectId;
        Title = title;
        Description = description;
        AssigneeUserId = assigneeUserId;
    }

    public Guid OrganizationId { get; private init; }

    public Guid ProjectId { get; private init; }

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public Guid? AssigneeUserId { get; private set; }

    public TaskStatus Status { get; private set; } = TaskStatus.Todo;

    public DateTimeOffset CreatedAtUtc { get; private init; }

    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public static TaskItem Create(
        Guid organizationId,
        Guid projectId,
        string title,
        string? description = null,
        Guid? assigneeUserId = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization ID cannot be empty.",
                nameof(organizationId));
        }

        if (projectId == Guid.Empty)
        {
            throw new ArgumentException(
                "Project ID cannot be empty.",
                nameof(projectId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        return new TaskItem(
            Guid.NewGuid(),
            organizationId,
            projectId,
            title.Trim(),
            string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim(),
            assigneeUserId)
        {
            CreatedAtUtc = DateTimeOffset.UtcNow
        };
    }

    public void AssignTo(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        AssigneeUserId = userId;
        Touch();
    }

    public void Unassign()
    {
        AssigneeUserId = null;
        Touch();
    }

    public void ChangeStatus(TaskStatus status)
    {
        Status = status;
        Touch();
    }

    public void Update(
        string title,
        string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
