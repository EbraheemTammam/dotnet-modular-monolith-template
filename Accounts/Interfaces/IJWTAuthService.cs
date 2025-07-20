using Base.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface IJWTAuthService
{
    Task<Response<LoginResponseDTO>> LoginAsync(LoginDTO loginDTO);
    Task<Response<LoginResponseDTO>> RefreshAsync(LoginResponseDTO tokenDTO);
    Task<Response> LogoutAsync();
}
