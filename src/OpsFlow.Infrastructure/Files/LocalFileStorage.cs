using Microsoft.Extensions.Hosting;
using OpsFlow.Application.Abstractions.Files;

namespace OpsFlow.Infrastructure.Files;

public sealed class LocalFileStorage(IHostEnvironment environment) : IFileStorage
{
    private string Root =>
        Path.Combine(environment.ContentRootPath, "storage", "files");

    public async Task<string> SaveAsync(
        Stream content,
        string storageKey,
        CancellationToken cancellationToken)
    {
        var fullPath = GetSafePath(storageKey);
        var directory = Path.GetDirectoryName(fullPath)!;

        Directory.CreateDirectory(directory);

        await using var output = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        await content.CopyToAsync(output, cancellationToken);

        return storageKey;
    }

    public Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var fullPath = GetSafePath(storageKey);

        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            64 * 1024,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken)
    {
        var fullPath = GetSafePath(storageKey);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private string GetSafePath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey) ||
            Path.IsPathRooted(storageKey) ||
            storageKey.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException("Invalid storage key.", nameof(storageKey));
        }

        var root = Path.GetFullPath(Root);
        var path = Path.GetFullPath(Path.Combine(root, storageKey));

        if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new ArgumentException("Invalid storage key.", nameof(storageKey));

        return path;
    }
}
