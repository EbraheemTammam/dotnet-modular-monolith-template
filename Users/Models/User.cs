using Microsoft.AspNetCore.Identity;

namespace Users.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Document? ProfilePicture { get; set; }
    public float WalletBalance { get; set; }
    public string GetFullName() => $"{FirstName} {LastName}";
}
