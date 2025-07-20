using Accounts.Models;

namespace Accounts.Interfaces;

public interface ICurrentLoggedInUser
{
    string UserId { get; }
    Task<User> GetUser();
}
