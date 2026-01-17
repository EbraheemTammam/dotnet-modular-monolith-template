namespace Accounts.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public required string Token { get; set; }
    public DateTime Expiry { get; set; }
    public bool IsRevoked { get; set; } = false;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
