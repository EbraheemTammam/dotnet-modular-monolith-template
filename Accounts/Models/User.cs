using Microsoft.AspNetCore.Identity;

using Base.Models;

namespace Accounts.Models;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public Guid? ProfilePictureId { get; set; }
    public Document? ProfilePicture { get; set; }
    public float WalletBalance { get; set; }
    public string GetFullName() => $"{FirstName} {LastName}";
}
