using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Base.Controllers;
using Accounts.DTOs;
using Accounts.Interfaces;

namespace Accounts.Controllers;

public class AuthController : ApiBaseController
{
    private readonly IUserService _userService;
    private readonly IJWTAuthService _jwtAuthService;
    public AuthController(IUserService userService, IJWTAuthService jwtAuthService)
    {
        _userService = userService;
        _jwtAuthService = jwtAuthService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login([FromForm] LoginDTO loginDTO) =>
        HandleResult(await _jwtAuthService.LoginAsync(loginDTO));

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDTO>> Refresh([FromForm] LoginResponseDTO tokenDTO) =>
        HandleResult(await _jwtAuthService.RefreshAsync(tokenDTO));

    [HttpPost("logout")]
    public async Task<ActionResult> Logout() =>
        HandleResult(await _jwtAuthService.LogoutAsync());

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromForm] UserAddDTO userAddDTO) =>
        HandleResult(await _userService.Register(userAddDTO, Url));

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([Required, EmailAddress] string email, [Required] string token) =>
        HandleResult(await _userService.ConfirmEmail(email, token));

    [HttpPost("verify-phone-number")]
    public async Task<ActionResult> VerifyPhoneNumber([Required, Phone] string phoneNumber, [Required] string token) =>
        HandleResult(await _userService.VerifyPhoneNumber(phoneNumber, token));

    [HttpPut("change-password"), Authorize]
    public async Task<ActionResult> ChangePassword(UserUpdatePasswordDTO userUpdatePasswordDTO) =>
        HandleResult(await _userService.UpdatePassword(userUpdatePasswordDTO));
}
