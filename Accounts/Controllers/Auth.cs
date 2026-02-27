using Microsoft.AspNetCore.Mvc;

using Shared.Controllers;
using Accounts.DTOs;
using Accounts.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Accounts.Controllers;

public class AuthController : ApiBaseController
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDTO loginDTO, CancellationToken ct) =>
        HandleResult(await _authService.LoginAsync(loginDTO, ct));

    [HttpPost("token-refresh")]
    public async Task<ActionResult> RefreshToken([FromBody, Required] string refreshToken, CancellationToken ct) =>
        HandleResult(await _authService.RefreshTokenAsync(refreshToken, ct));

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody, Required] string refreshToken, CancellationToken ct) =>
        HandleResult(await _authService.LogoutAsync(refreshToken, ct));
}
