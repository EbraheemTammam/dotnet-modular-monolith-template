namespace Accounts.Models;

public class Verification
{
    public required string PhoneNumber { get; set; }
    public required string Token { get; set; }
}
