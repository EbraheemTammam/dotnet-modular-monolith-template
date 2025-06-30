using Base.Responses;
using Auth.DTOs;

namespace Auth.Interfaces;

public interface ICookieAuthService
{
    Task<Response> LoginAsync(LoginDTO loginDTO);
    Task<Response> LogoutAsync();
}
