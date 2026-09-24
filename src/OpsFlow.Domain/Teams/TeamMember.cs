using OpsFlow.Domain.Common;

namespace OpsFlow.Domain.Teams;

public sealed class TeamMember : Entity
{
    private TeamMember()
    {
    }

    private TeamMember(
        Guid id,
        Guid teamId,
        Guid membershipId)
    {
        Id = id;
        TeamId = teamId;
        MembershipId = membershipId;
        AddedAtUtc = DateTimeOffset.UtcNow;
    }

    public Guid TeamId { get; private init; }

    public Guid MembershipId { get; private init; }

    public DateTimeOffset AddedAtUtc { get; private init; }

    public static TeamMember Create(Guid teamId, Guid membershipId)
    {
        if (teamId == Guid.Empty)
        {
            throw new ArgumentException("Team ID is required.", nameof(teamId));
        }

        if (membershipId == Guid.Empty)
        {
            throw new ArgumentException("Membership ID is required.", nameof(membershipId));
        }

        return new TeamMember(
            Guid.NewGuid(),
            teamId,
            membershipId);
    }
}
