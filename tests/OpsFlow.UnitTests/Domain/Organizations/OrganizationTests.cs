using FluentAssertions;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.UnitTests.Domain.Organizations;

public sealed class OrganizationTests
{
    [Fact]
    public void CreateShouldNormalizeSlug()
    {
        var organization = Organization.Create(
            "OpsFlow Inc.",
            "  OpsFlow  ");

        organization.Name.Should().Be("OpsFlow Inc.");
        organization.Slug.Should().Be("opsflow");
        organization.IsActive.Should().BeTrue();
        organization.Id.Should().NotBeEmpty();
        organization.CreatedAtUtc.Should().NotBe(default);
    }

    [Fact]
    public void DeactivateShouldMakeOrganizationInactive()
    {
        var organization = Organization.Create(
            "OpsFlow Inc.",
            "opsflow");

        organization.Deactivate();

        organization.IsActive.Should().BeFalse();
    }

    [Fact]
    public void ActivateShouldMakeOrganizationActive()
    {
        var organization = Organization.Create(
            "OpsFlow Inc.",
            "opsflow");

        organization.Deactivate();
        organization.Activate();

        organization.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CreateShouldRejectEmptyName()
    {
        var action = () => Organization.Create(
            "",
            "opsflow");

        action.Should()
            .Throw<ArgumentException>();
    }
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateShouldRejectBlankSlug(string slug)
    {
        var action = () => Organization.Create(
            "OpsFlow Inc.",
            slug);

        action.Should()
            .Throw<ArgumentException>();
    }

    [Fact]
    public void CreateShouldTrimName()
    {
        var organization = Organization.Create(
            "  OpsFlow Inc.  ",
            "opsflow");

        organization.Name.Should().Be("OpsFlow Inc.");
    }

}

