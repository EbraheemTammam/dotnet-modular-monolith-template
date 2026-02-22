using Microsoft.AspNetCore.Identity;

namespace Accounts.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly RegisteredAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string FullName => $"{FirstName} {LastName}";
}
