using Auth.Models;

namespace Auth.Interfaces;

public interface ICurrentLoggedInUser
{
    string UserId { get; }
    Task<User> GetUser();
}
