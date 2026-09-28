using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Api.Contracts.Identity;

public sealed record RegisterUserRequest(
    [EmailAddress]
    string Email,
    string FirstName,
    string LastName,
    string Password);
