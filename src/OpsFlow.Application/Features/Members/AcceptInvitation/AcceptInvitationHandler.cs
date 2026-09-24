using OpsFlow.Application.Abstractions.Identity;
using OpsFlow.Application.Abstractions.Persistence;
using OpsFlow.Application.Common.Exceptions;
using OpsFlow.Application.Features.Members.Common;
using OpsFlow.Domain.Organizations;

namespace OpsFlow.Application.Features.Members.AcceptInvitation;

public sealed class AcceptInvitationHandler(
    ICurrentUser currentUser,
    IUserRepository userRepository,
    IOrganizationInvitationRepository invitationRepository,
    IMembershipRepository membershipRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<AcceptInvitationResult> HandleAsync(
        AcceptInvitationCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Token);

        var invitation = await invitationRepository.GetPendingByTokenHashAsync(
            InvitationToken.Hash(command.Token),
            cancellationToken);

        if (invitation is null)
        {
            throw new NotFoundException("Invitation was not found.");
        }

        if (invitation.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            throw new ConflictException("The invitation has expired.");
        }

        var user = await userRepository.GetByNormalizedEmailAsync(
            invitation.NormalizedEmail,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new ConflictException(
                "The invited email does not belong to an active user.");
        }

        if (user.Id != currentUser.UserId)
        {
            throw new ForbiddenException(
                "The invitation belongs to a different user.");
        }

        if (await membershipRepository.IsActiveMemberAsync(
                invitation.OrganizationId,
                user.Id,
                cancellationToken))
        {
            throw new ConflictException(
                "The user is already an active member of the organization.");
        }

        invitation.Accept();

        var membership = Membership.Create(
            invitation.OrganizationId,
            user.Id,
            invitation.RoleId);

        await membershipRepository.AddAsync(
            membership,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AcceptInvitationResult(
            membership.Id,
            membership.OrganizationId,
            membership.UserId,
            membership.RoleId);
    }
}
