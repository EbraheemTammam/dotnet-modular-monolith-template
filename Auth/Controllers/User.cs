using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Base.Controllers;
using Users.DTOs;
using Auth.Interfaces;

namespace Auth.Controllers;

public class UsersController : ApiBaseController
{
    private readonly IExtendedUserService _userService;
    public UsersController(IExtendedUserService userService) =>
        _userService = userService;

    [HttpGet("profile"), Authorize]
    public async Task<ActionResult<UserDTO>> GetProfile() =>
        HandleResult(await _userService.GetUser(
            new Guid(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!)
        ));

    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser([FromForm] UserAddDTO userAddDTO) =>
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

    [HttpPatch("update_profile"), Authorize]
    public async Task<ActionResult<UserDTO>> PartialUpdateUser([FromForm] UserPartialUpdateDTO userPartialUpdateDTO) =>
        HandleResult(await _userService.PartialUpdateUser(userPartialUpdateDTO));

    [HttpPut("change_password"), Authorize]
    public async Task<ActionResult> ChangePassword(UserUpdatePasswordDTO userUpdatePasswordDTO) =>
        HandleResult(await _userService.UpdatePassword(userUpdatePasswordDTO));
}
