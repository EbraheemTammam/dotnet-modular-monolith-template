using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Users.Models;

namespace Users.DTOs;

public record UserAddDTO
{
    [Required, EmailAddress, MaxLength(150)] public required string Email { get; init; }
    [Required, MaxLength(100)] public required string Password { get; init; }
    [Required, MaxLength(20)] public required string FirstName { get; init; }
    [Required, MaxLength(20)] public required string LastName { get; init; }
    [Required, MaxLength(13)] public required string PhoneNumber { get; init; }
    public IFormFile? ProfilePicture { get; init; }
    public User ToModel() =>
        new User
        {
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            PhoneNumber = PhoneNumber,
        };
}

public record UserPartialUpdateDTO
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public IFormFile? ProfilePicture { get; init; }
}

public record UserUpdatePasswordDTO
{
    [Required] public required string CurrentPassword { get; init; }
    [Required] public required string NewPassword { get; init; }
}

public record UserDTO
{
    public Guid Id { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public static UserDTO FromModel(User user, HttpRequest request) =>
        new UserDTO
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber!,
            ProfilePictureUrl = $"{request.Scheme}://{request.Host.Value}/{user.ProfilePicture?.Url}"
        };
}
