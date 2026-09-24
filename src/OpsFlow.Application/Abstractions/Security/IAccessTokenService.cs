namespace OpsFlow.Application.Abstractions.Security;

public interface IAccessTokenService
{
    string CreateToken(Guid userId);
}
