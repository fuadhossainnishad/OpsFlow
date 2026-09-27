using System.Text;
using FluentAssertions;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Files;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Features.Files.UploadFile;
using OpsFlow.Domain.Files;

namespace OpsFlow.UnitTests.Features.Files.UploadFile;

public sealed class UploadFileHandlerTests
{
    private static readonly Guid OrganizationId =
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid UserId =
        Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task HandleShouldUploadCreateRecordAuditAndSave()
    {
        var repository = new FakeFileRepository();
        var storage = new FakeFileStorage();
        var auditLogger = new FakeAuditLogger();
        var unitOfWork = new FakeUnitOfWork();

        var handler = CreateHandler(
            repository,
            storage,
            auditLogger,
            unitOfWork);

        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("hello opsflow"));

        var result = await handler.HandleAsync(
            content,
            "  ../invoice.pdf  ",
            " application/pdf ",
            12,
            CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.OrganizationId.Should().Be(OrganizationId);
        result.UploadedByUserId.Should().Be(UserId);
        result.OriginalFileName.Should().Be("invoice.pdf");
        result.ContentType.Should().Be("application/pdf");
        result.SizeBytes.Should().Be(12);

        repository.AddedFile.Should().NotBeNull();
        repository.AddedFile!.Id.Should().Be(result.Id);
        repository.AddedFile.OriginalFileName.Should().Be("invoice.pdf");
        repository.AddedFile.OrganizationId.Should().Be(OrganizationId);
        repository.AddedFile.UploadedByUserId.Should().Be(UserId);

        storage.SavedContent.Should().Be("hello opsflow");
        storage.SavedStorageKey.Should().NotBeNullOrWhiteSpace();
        storage.SavedStorageKey.Should().StartWith($"{OrganizationId:N}/");

        auditLogger.Action.Should().Be("file.uploaded");
        auditLogger.Resource.Should().Be("file");
        auditLogger.ResourceId.Should().Be(result.Id);
        auditLogger.OrganizationId.Should().Be(OrganizationId);
        auditLogger.ActorUserId.Should().Be(UserId);
        auditLogger.AfterJson.Should().Contain("\"fileName\":\"invoice.pdf\"");
        auditLogger.AfterJson.Should().Contain("\"sizeBytes\":12");

        unitOfWork.SaveCalls.Should().Be(1);
        storage.DeleteCalls.Should().Be(0);
    }

    [Fact]
    public async Task HandleShouldRejectEmptyFile()
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            new MemoryStream(),
            "file.txt",
            "text/plain",
            0,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*must not be empty*");
    }

    [Fact]
    public async Task HandleShouldRejectFileOver25Mb()
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            new MemoryStream(),
            "file.txt",
            "text/plain",
            25L * 1024 * 1024 + 1,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*cannot exceed 25 MB*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleShouldRejectMissingFileName(string fileName)
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            new MemoryStream(),
            fileName,
            "text/plain",
            10,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleShouldRejectMissingContentType(string contentType)
    {
        var handler = CreateHandler();

        var action = () => handler.HandleAsync(
            new MemoryStream(),
            "file.txt",
            contentType,
            10,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task HandleShouldDeleteStorageWhenRepositoryFails()
    {
        var repository = new FakeFileRepository
        {
            AddException = new InvalidOperationException("database failure")
        };
        var storage = new FakeFileStorage();

        var handler = CreateHandler(
            repository,
            storage);

        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("content"));

        var action = () => handler.HandleAsync(
            content,
            "file.txt",
            "text/plain",
            7,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("database failure");

        storage.SavedStorageKey.Should().NotBeNullOrWhiteSpace();
        storage.DeletedStorageKey.Should().Be(storage.SavedStorageKey);
        storage.DeleteCalls.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldDeleteStorageWhenAuditFails()
    {
        var storage = new FakeFileStorage();
        var auditLogger = new FakeAuditLogger
        {
            LogException = new InvalidOperationException("audit failure")
        };

        var handler = CreateHandler(
            storage: storage,
            auditLogger: auditLogger);

        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("content"));

        var action = () => handler.HandleAsync(
            content,
            "file.txt",
            "text/plain",
            7,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("audit failure");

        storage.DeletedStorageKey.Should().Be(storage.SavedStorageKey);
        storage.DeleteCalls.Should().Be(1);
    }

    [Fact]
    public async Task HandleShouldDeleteStorageWhenSaveChangesFails()
    {
        var storage = new FakeFileStorage();
        var unitOfWork = new FakeUnitOfWork
        {
            SaveException = new InvalidOperationException("save failure")
        };

        var handler = CreateHandler(
            storage: storage,
            unitOfWork: unitOfWork);

        await using var content =
            new MemoryStream(Encoding.UTF8.GetBytes("content"));

        var action = () => handler.HandleAsync(
            content,
            "file.txt",
            "text/plain",
            7,
            CancellationToken.None);

        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("save failure");

        storage.DeletedStorageKey.Should().Be(storage.SavedStorageKey);
        storage.DeleteCalls.Should().Be(1);
    }

    private static UploadFileHandler CreateHandler(
        FakeFileRepository? repository = null,
        FakeFileStorage? storage = null,
        FakeAuditLogger? auditLogger = null,
        FakeUnitOfWork? unitOfWork = null)
    {
        return new UploadFileHandler(
            repository ?? new FakeFileRepository(),
            storage ?? new FakeFileStorage(),
            new FakeCurrentUser(),
            new FakeTenantContext(),
            auditLogger ?? new FakeAuditLogger(),
            unitOfWork ?? new FakeUnitOfWork());
    }

    private sealed class FakeFileRepository : IFileRepository
    {
        public FileRecord? AddedFile { get; private set; }
        public Exception? AddException { get; init; }

        public Task AddAsync(
            FileRecord file,
            CancellationToken cancellationToken)
        {
            if (AddException is not null)
                throw AddException;

            AddedFile = file;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<FileRecord>> ListAsync(
            Guid organizationId,
            int skip,
            int take,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<FileRecord>>([]);

        public Task<FileRecord?> GetAsync(
            Guid organizationId,
            Guid fileId,
            CancellationToken cancellationToken) =>
            Task.FromResult<FileRecord?>(null);

        public Task DeleteAsync(
            FileRecord file,
            CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class FakeFileStorage : IFileStorage
    {
        public string? SavedStorageKey { get; private set; }
        public string? DeletedStorageKey { get; private set; }
        public string? SavedContent { get; private set; }
        public int DeleteCalls { get; private set; }

        public async Task<string> SaveAsync(
            Stream content,
            string storageKey,
            CancellationToken cancellationToken)
        {
            SavedStorageKey = storageKey;

            using var reader = new StreamReader(
                content,
                leaveOpen: true);

            SavedContent = await reader.ReadToEndAsync(cancellationToken);

            return storageKey;
        }

        public Task<Stream?> OpenReadAsync(
            string storageKey,
            CancellationToken cancellationToken) =>
            Task.FromResult<Stream?>(null);

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken)
        {
            DeletedStorageKey = storageKey;
            DeleteCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAuditLogger : IAuditLogger
    {
        public Guid OrganizationId { get; private set; }
        public Guid? ActorUserId { get; private set; }
        public string? Action { get; private set; }
        public string? Resource { get; private set; }
        public Guid? ResourceId { get; private set; }
        public string? AfterJson { get; private set; }
        public Exception? LogException { get; init; }

        public Task LogAsync(
            Guid organizationId,
            Guid? actorUserId,
            string action,
            string resource,
            Guid? resourceId,
            string? beforeJson,
            string? afterJson,
            CancellationToken cancellationToken)
        {
            if (LogException is not null)
                throw LogException;

            OrganizationId = organizationId;
            ActorUserId = actorUserId;
            Action = action;
            Resource = resource;
            ResourceId = resourceId;
            AfterJson = afterJson;

            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCalls { get; private set; }
        public Exception? SaveException { get; init; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken)
        {
            SaveCalls++;

            if (SaveException is not null)
                throw SaveException;

            return Task.FromResult(1);
        }
    }

    private sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid UserId => UploadFileHandlerTests.UserId;
    }

    private sealed class FakeTenantContext : ITenantContext
    {
        public Task<Guid> GetOrganizationIdAsync(
            CancellationToken cancellationToken) =>
            Task.FromResult(UploadFileHandlerTests.OrganizationId);
    }
}
