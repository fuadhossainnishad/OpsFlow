using FluentAssertions;
using OpsFlow.Domain.Projects;

namespace OpsFlow.UnitTests.Domain.Projects;

public sealed class ProjectTests
{
    [Fact]
    public void CreateShouldNormalizeProject()
    {
        var organizationId = Guid.NewGuid();

        var project = Project.Create(
            organizationId,
            "  My Project  ",
            "  abc-123  ",
            "  Project description  ");

        project.Id.Should().NotBeEmpty();
        project.OrganizationId.Should().Be(organizationId);
        project.Name.Should().Be("My Project");
        project.Key.Should().Be("ABC-123");
        project.Description.Should().Be("Project description");
        project.Status.Should().Be(ProjectStatus.Active);
        project.ArchivedAtUtc.Should().BeNull();
    }

    [Fact]
    public void CreateShouldRejectEmptyOrganizationId()
    {
        var action = () => Project.Create(
            Guid.Empty,
            "Project",
            "PROJ",
            null);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldRejectNameOver200Characters()
    {
        var action = () => Project.Create(
            Guid.NewGuid(),
            new string('A', 201),
            "PROJ",
            null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Fact]
    public void CreateShouldRejectKeyOver50Characters()
    {
        var action = () => Project.Create(
            Guid.NewGuid(),
            "Project",
            new string('A', 51),
            null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*cannot exceed 50 characters*");
    }

    [Fact]
    public void CreateShouldConvertBlankDescriptionToNull()
    {
        var project = Project.Create(
            Guid.NewGuid(),
            "Project",
            "PROJ",
            "   ");

        project.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateShouldTrimValues()
    {
        var project = CreateProject();

        project.Update(
            "  Updated Project  ",
            "  Updated description  ");

        project.Name.Should().Be("Updated Project");
        project.Description.Should().Be("Updated description");
    }

    [Fact]
    public void UpdateShouldConvertBlankDescriptionToNull()
    {
        var project = CreateProject();

        project.Update("Updated", "   ");

        project.Description.Should().BeNull();
    }

    [Fact]
    public void UpdateShouldRejectNameOver200Characters()
    {
        var project = CreateProject();

        var action = () => project.Update(
            new string('A', 201),
            null);

        action.Should()
            .Throw<ArgumentException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Fact]
    public void ArchiveShouldArchiveProject()
    {
        var project = CreateProject();

        project.Archive();

        project.Status.Should().Be(ProjectStatus.Archived);
        project.ArchivedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void ArchiveShouldBeIdempotent()
    {
        var project = CreateProject();

        project.Archive();
        var archivedAt = project.ArchivedAtUtc;

        project.Archive();

        project.ArchivedAtUtc.Should().Be(archivedAt);
    }

    [Fact]
    public void UpdateShouldRejectArchivedProject()
    {
        var project = CreateProject();
        project.Archive();

        var action = () => project.Update(
            "Updated",
            null);

        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Archived projects cannot be updated.");
    }

    private static Project CreateProject()
    {
        return Project.Create(
            Guid.NewGuid(),
            "Project",
            "PROJ",
            null);
    }
}
