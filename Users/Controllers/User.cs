using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Base.Controllers;
using Users.DTOs;
using Users.Interfaces;

namespace Users.Controllers;

public class UsersController : ApiBaseController
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) =>
        _userService = userService;

    [HttpGet, Authorize(Roles = "superuser")]
    public ActionResult<IEnumerable<UserDTO>> GetAllUsers() =>
        HandleResult(_userService.GetAllUsers());

    [HttpGet("{id}"), Authorize(Roles = "superuser")]
    public async Task<ActionResult<UserDTO>> GetUser(Guid id) =>
        HandleResult(await _userService.GetUser(id));

    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser([FromForm] UserAddDTO userAddDTO) =>
        HandleResult(await _userService.AddUser(userAddDTO));

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

    [HttpDelete("{id}"), Authorize(Roles = "superuser")]
    public async Task<ActionResult> DeleteUser(Guid id) =>
        HandleResult(await _userService.DeleteUser(id));
}
