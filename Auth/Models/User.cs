using Microsoft.AspNetCore.Identity;

namespace Auth.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string GetFullName() => $"{FirstName} {LastName}";
}
