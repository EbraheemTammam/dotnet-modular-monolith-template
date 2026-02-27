using Microsoft.AspNetCore.Identity;

namespace Accounts.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string FullName => $"{FirstName} {LastName}";
    public ICollection<IdentityRole<Guid>>? Roles;
}
