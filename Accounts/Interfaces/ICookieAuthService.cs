using Base.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface ICookieAuthService
{
    Task<Response> LoginAsync(LoginDTO loginDTO);
    Task<Response> LogoutAsync();
}
