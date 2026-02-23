using Shared.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface IAuthService
{
    Task<Response> LoginAsync(LoginDTO loginDTO);
    Task<Response> LogoutAsync();
}
