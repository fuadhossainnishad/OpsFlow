using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Abstractions.Tenancy;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Members.Common;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Features.Members.InviteMember;

public sealed class InviteMemberHandler(
    ITenantContext tenantContext,
    IOrganizationInvitationRepository invitationRepository,
    IMembershipRepository membershipRepository,
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<InviteMemberResult> HandleAsync(
        InviteMemberCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var organizationId =
            await tenantContext.GetOrganizationIdAsync(cancellationToken);

        ArgumentException.ThrowIfNullOrWhiteSpace(command.Email);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.RoleName);

        var email = command.Email.Trim();
        var normalizedEmail = email.ToUpperInvariant();
        var normalizedRoleName = command.RoleName.Trim().ToUpperInvariant();

        var role = await roleRepository.GetByNormalizedNameAsync(
            normalizedRoleName,
            cancellationToken);

        if (role is null || !role.IsSystemRole)
        {
            throw new NotFoundException("The requested role was not found.");
        }

        var existingUser = await userRepository.GetByNormalizedEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (existingUser is not null &&
            await membershipRepository.IsActiveMemberAsync(
                organizationId,
                existingUser.Id,
                cancellationToken))
        {
            throw new ConflictException(
                "The user is already an active member of the organization.");
        }

        if (await invitationRepository.HasPendingInvitationAsync(
                organizationId,
                normalizedEmail,
                cancellationToken))
        {
            throw new ConflictException(
                "A pending invitation already exists for this email.");
        }

        var token = InvitationToken.Generate();

        var invitation = OrganizationInvitation.Create(
            organizationId,
            email,
            role.Id,
            InvitationToken.Hash(token),
            DateTimeOffset.UtcNow.AddDays(7));

        await invitationRepository.AddAsync(
            invitation,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InviteMemberResult(
            invitation.Id,
            invitation.OrganizationId,
            invitation.Email,
            role.Name,
            invitation.ExpiresAtUtc,
            token);
    }
}
