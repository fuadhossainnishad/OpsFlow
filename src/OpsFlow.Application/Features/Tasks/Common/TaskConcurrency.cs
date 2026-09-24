using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Domain.Tasks;

namespace OpsFlow.Application.Features.Tasks.Common;

internal static class TaskConcurrency
{
    public static void EnsureCurrent(
        TaskItem task,
        string rowVersion)
    {
        if (string.IsNullOrWhiteSpace(rowVersion))
        {
            throw new ConflictException(
                "A task row version is required.");
        }

        byte[] requestedVersion;

        try
        {
            requestedVersion = Convert.FromBase64String(rowVersion);
        }
        catch (FormatException)
        {
            throw new ConflictException(
                "The task row version is invalid.");
        }

        if (!requestedVersion.SequenceEqual(task.RowVersion))
        {
            throw new ConflictException(
                "The task was modified by another request. Reload it and try again.");
        }
    }
}
