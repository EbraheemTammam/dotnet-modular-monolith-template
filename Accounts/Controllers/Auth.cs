using Microsoft.AspNetCore.Mvc;

using Shared.Controllers;
using Accounts.DTOs;
using Accounts.Interfaces;

namespace Accounts.Controllers;

public class AuthController : ApiBaseController
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDTO loginDTO) =>
        HandleResult(await _authService.LoginAsync(loginDTO));

    [HttpPost("logout")]
    public async Task<ActionResult> Logout() =>
        HandleResult(await _authService.LogoutAsync());
}
