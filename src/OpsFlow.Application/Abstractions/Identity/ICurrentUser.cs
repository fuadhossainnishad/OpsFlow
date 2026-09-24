namespace OpsFlow.Application.Abstractions.Identity;

public interface ICurrentUser
{
    Guid UserId { get; }
}
