using Shared.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface IAuthService
{
    Task<Response> LoginAsync(LoginDTO loginDTO, CancellationToken ct = default);
    Task<Response> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<Response> LogoutAsync(string? refreshToken = default, CancellationToken ct = default);
}
