using Base.Responses;
using Auth.DTOs;

namespace Auth.Interfaces;

public interface IJWTAuthService
{
    Task<Response<LoginResponseDTO>> LoginAsync(LoginDTO loginDTO);
    Task<Response<LoginResponseDTO>> RefreshAsync(LoginResponseDTO tokenDTO);
    Task<Response> LogoutAsync();
}
