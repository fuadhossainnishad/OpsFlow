using System.Text;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using OpsFlow.Infrastructure.Files;

namespace OpsFlow.UnitTests.Infrastructure.Files;

public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _root;
    private readonly LocalFileStorage _storage;

    public LocalFileStorageTests()
    {
        _root = Path.Combine(
            Path.GetTempPath(),
            $"opsflow-file-tests-{Guid.NewGuid():N}");

        Directory.CreateDirectory(_root);

        var environment = new TestHostEnvironment
        {
            ContentRootPath = _root
        };

        _storage = new LocalFileStorage(environment);
    }

    [Fact]
    public async Task SaveAsyncShouldPersistContent()
    {
        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("hello opsflow"));

        var result = await _storage.SaveAsync(
            content,
            "documents/test.txt",
            CancellationToken.None);

        result.Should().Be("documents/test.txt");

        var path = Path.Combine(
            _root,
            "storage",
            "files",
            "documents",
            "test.txt");

        File.Exists(path).Should().BeTrue();

        var savedContent = await File.ReadAllTextAsync(path);

        savedContent.Should().Be("hello opsflow");
    }

    [Fact]
    public async Task OpenReadAsyncShouldReturnStoredContent()
    {
        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("read me"));

        await _storage.SaveAsync(
            content,
            "documents/read.txt",
            CancellationToken.None);

        await using var stream = await _storage.OpenReadAsync(
            "documents/read.txt",
            CancellationToken.None);

        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream!);

        var result = await reader.ReadToEndAsync();

        result.Should().Be("read me");
    }

    [Fact]
    public async Task OpenReadAsyncShouldReturnNullWhenFileDoesNotExist()
    {
        var result = await _storage.OpenReadAsync(
            "missing.txt",
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsyncShouldDeleteExistingFile()
    {
        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("delete me"));

        await _storage.SaveAsync(
            content,
            "documents/delete.txt",
            CancellationToken.None);

        await _storage.DeleteAsync(
            "documents/delete.txt",
            CancellationToken.None);

        var result = await _storage.OpenReadAsync(
            "documents/delete.txt",
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsyncShouldDoNothingWhenFileDoesNotExist()
    {
        var action = () => _storage.DeleteAsync(
            "missing.txt",
            CancellationToken.None);

        await action.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("../outside.txt")]
    [InlineData("folder/../outside.txt")]
    public async Task OperationsShouldRejectInvalidStorageKeys(
        string storageKey)
    {
        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("test"));

        var save = () => _storage.SaveAsync(
            content,
            storageKey,
            CancellationToken.None);

        await save.Should().ThrowAsync<ArgumentException>();

        var read = () => _storage.OpenReadAsync(
            storageKey,
            CancellationToken.None);

        await read.Should().ThrowAsync<ArgumentException>();

        var delete = () => _storage.DeleteAsync(
            storageKey,
            CancellationToken.None);

        await delete.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task OperationsShouldRejectRootedStorageKey()
    {
        var rootedPath = Path.Combine(
            Path.GetTempPath(),
            "outside.txt");

        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("test"));

        var save = () => _storage.SaveAsync(
            content,
            rootedPath,
            CancellationToken.None);

        await save.Should().ThrowAsync<ArgumentException>();

        var read = () => _storage.OpenReadAsync(
            rootedPath,
            CancellationToken.None);

        await read.Should().ThrowAsync<ArgumentException>();

        var delete = () => _storage.DeleteAsync(
            rootedPath,
            CancellationToken.None);

        await delete.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SaveAsyncShouldRespectCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("cancelled"));

        var action = () => _storage.SaveAsync(
            content,
            "cancelled.txt",
            cancellation.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task SaveAsyncShouldRejectDuplicateStorageKey()
    {
        await using var first =
            new MemoryStream(Encoding.UTF8.GetBytes("first"));

        await _storage.SaveAsync(
            first,
            "duplicate.txt",
            CancellationToken.None);

        await using var second =
            new MemoryStream(Encoding.UTF8.GetBytes("second"));

        var action = () => _storage.SaveAsync(
            second,
            "duplicate.txt",
            CancellationToken.None);

        await action.Should().ThrowAsync<IOException>();
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Testing";

        public string ApplicationName { get; set; } =
            typeof(LocalFileStorageTests).Assembly.GetName().Name!;

        public string ContentRootPath { get; set; } = null!;

        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}
