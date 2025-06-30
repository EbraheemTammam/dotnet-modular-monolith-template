using System.ComponentModel.DataAnnotations;

namespace Auth.DTOs;

public record LoginDTO
{
    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
