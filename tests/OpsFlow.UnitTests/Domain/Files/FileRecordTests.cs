using FluentAssertions;
using OpsFlow.Domain.Files;

namespace OpsFlow.UnitTests.Domain.Files;

public sealed class FileRecordTests
{
    [Fact]
    public void CreateShouldInitializeAllProperties()
    {
        var organizationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        var result = FileRecord.Create(
            organizationId,
            userId,
            " report.pdf ",
            " files/report.pdf ",
            " application/pdf ",
            1024,
            createdAt);

        result.Id.Should().NotBeEmpty();
        result.OrganizationId.Should().Be(organizationId);
        result.UploadedByUserId.Should().Be(userId);
        result.OriginalFileName.Should().Be("report.pdf");
        result.StorageKey.Should().Be("files/report.pdf");
        result.ContentType.Should().Be("application/pdf");
        result.SizeBytes.Should().Be(1024);
        result.CreatedAtUtc.Should().Be(createdAt);
    }

    [Fact]
    public void CreateShouldExtractFileNameFromPath()
    {
        var result = FileRecord.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "/uploads/documents/report.pdf",
            "files/report.pdf",
            "application/pdf",
            100,
            DateTimeOffset.UtcNow);

        result.OriginalFileName.Should().Be("report.pdf");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankOriginalFileName(string fileName)
    {
        var act = () => FileRecord.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            fileName,
            "files/test.pdf",
            "application/pdf",
            100,
            DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("originalFileName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankStorageKey(string storageKey)
    {
        var act = () => FileRecord.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test.pdf",
            storageKey,
            "application/pdf",
            100,
            DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(storageKey));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankContentType(string contentType)
    {
        var act = () => FileRecord.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test.pdf",
            "files/test.pdf",
            contentType,
            100,
            DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(contentType));
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var act = () => FileRecord.Create(
            Guid.Empty,
            Guid.NewGuid(),
            "test.pdf",
            "files/test.pdf",
            "application/pdf",
            100,
            DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("organizationId");
    }

    [Fact]
    public void CreateShouldRejectEmptyUploaderUserId()
    {
        var act = () => FileRecord.Create(
            Guid.NewGuid(),
            Guid.Empty,
            "test.pdf",
            "files/test.pdf",
            "application/pdf",
            100,
            DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("uploadedByUserId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateShouldRejectNonPositiveSize(long sizeBytes)
    {
        var act = () => FileRecord.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test.pdf",
            "files/test.pdf",
            "application/pdf",
            sizeBytes,
            DateTimeOffset.UtcNow);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(sizeBytes));
    }
}
