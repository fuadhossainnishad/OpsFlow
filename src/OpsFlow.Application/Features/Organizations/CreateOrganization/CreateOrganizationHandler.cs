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

        var slugExists = await organizationRepository
            .ExistsBySlugAsync(
                slug,
                cancellationToken);

        if (slugExists)
        {
            throw new ConflictException(
                "An organization with this slug already exists.");
        }

        var organization = Organization.Create(
            name,
            slug);

        var ownerRoleId = await GetOwnerRoleIdAsync(
            cancellationToken);

        var membership = Membership.Create(
            organization.Id,
            currentUser.UserId,
            ownerRoleId);

        await organizationRepository.AddAsync(
            organization,
            cancellationToken);

        await membershipRepository.AddAsync(
            membership,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CreateOrganizationResult(
            organization.Id,
            organization.Name,
            organization.Slug);
    }

    private async Task<Guid> GetOwnerRoleIdAsync(
        CancellationToken cancellationToken)
    {
        var ownerRole = await roleRepository
            .GetByNormalizedNameAsync(
                "OWNER",
                cancellationToken);

        if (ownerRole is null || !ownerRole.IsSystemRole)
        {
            throw new InvalidOperationException(
                "The system Owner role is not configured.");
        }

        return ownerRole.Id;
    }
}
