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

    [HttpPatch("update_profile"), Authorize]
    public async Task<ActionResult<UserDTO>> PartialUpdateUser([FromForm] UserPartialUpdateDTO userPartialUpdateDTO) =>
        HandleResult(await _userService.PartialUpdateUser(userPartialUpdateDTO));
}
