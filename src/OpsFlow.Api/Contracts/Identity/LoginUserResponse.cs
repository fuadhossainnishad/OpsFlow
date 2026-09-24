namespace OpsFlow.Api.Contracts.Identity;

public sealed record LoginUserResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken);
