namespace OpsFlow.Application.Features.Organizations.CreateOrganization;

public sealed record CreateOrganizationCommand(
    string Name,
    string Slug);
