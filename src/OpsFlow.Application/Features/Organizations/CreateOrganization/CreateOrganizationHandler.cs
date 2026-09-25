using System.Text.Json;
using OpsFlow.Application.Abstractions.Auditing;
using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Features.Organizations.CreateOrganization;

public sealed class CreateOrganizationHandler(
    ICurrentUser currentUser,
    IOrganizationRepository organizationRepository,
    IMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    IAuditLogger auditLogger,
    IUnitOfWork unitOfWork)
{
    public async Task<CreateOrganizationResult> HandleAsync(
        CreateOrganizationCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var name = command.Name.Trim();
        var slug = command.Slug.Trim().ToLowerInvariant();

        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);

        if (await organizationRepository.ExistsBySlugAsync(slug, cancellationToken))
        {
            throw new ConflictException(
                "An organization with this slug already exists.");
        }

        var organization = Organization.Create(name, slug);

        var ownerRole = await roleRepository.GetByNormalizedNameAsync(
            "OWNER",
            cancellationToken);

        if (ownerRole is null || !ownerRole.IsSystemRole)
        {
            throw new InvalidOperationException(
                "The system Owner role is not configured.");
        }

        var membership = Membership.Create(
            organization.Id,
            currentUser.UserId,
            ownerRole.Id);

        await organizationRepository.AddAsync(organization, cancellationToken);
        await membershipRepository.AddAsync(membership, cancellationToken);

        await auditLogger.LogAsync(
            organization.Id,
            currentUser.UserId,
            "organization.created",
            "organization",
            organization.Id,
            null,
            JsonSerializer.Serialize(new
            {
                organization.Id,
                organization.Name,
                organization.Slug
            }),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrganizationResult(
            organization.Id,
            organization.Name,
            organization.Slug);
    }
}
