using System.ComponentModel.DataAnnotations;

namespace Accounts.DTOs;

public record LoginDTO
{
    [Required, EmailAddress] public required string Email { get; init; }
    [Required] public required string Password { get; init; }
}

public record TokenDTO
{
    [Required] public required string AccessToken { get; init; }
    [Required] public required string RefreshToken { get; init; }
}
