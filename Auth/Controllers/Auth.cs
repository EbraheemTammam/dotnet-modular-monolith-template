using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Base.Controllers;
using Users.DTOs;
using Auth.Interfaces;
using Auth.DTOs;

namespace Auth.Controllers;

public class AuthController : ApiBaseController
{
    private readonly IExtendedUserService _userService;
    private readonly IJWTAuthService _jwtAuthService;
    public AuthController(IExtendedUserService userService, IJWTAuthService jwtAuthService)
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
        HandleResult(await _userService.Register(userAddDTO));

    [HttpPost("send_email_verification")]
    public async Task<ActionResult> SendEmailVerification([Required, EmailAddress] string email) =>
        HandleResult(await _userService.SendEmailVerification(email));

    [HttpPost("send_phone_number_verification")]
    public async Task<ActionResult> SendPhoneNumberVerification([Required, Phone] string phoneNumber) =>
        HandleResult(await _userService.SendPhoneNumberVerification(phoneNumber));

    [HttpPost("verify_email")]
    public async Task<ActionResult> VerifyEmail([Required, EmailAddress] string email, [Required] string token) =>
        HandleResult(await _userService.VerifyEmail(email, token));

    [HttpPost("verify_phone_number")]
    public async Task<ActionResult> VerifyPhoneNumber([Required, Phone] string phoneNumber, [Required] string token) =>
        HandleResult(await _userService.VerifyPhoneNumber(phoneNumber, token));

    [HttpPut("change_password"), Authorize]
    public async Task<ActionResult> ChangePassword(UserUpdatePasswordDTO userUpdatePasswordDTO) =>
        HandleResult(await _userService.UpdatePassword(userUpdatePasswordDTO));
}
