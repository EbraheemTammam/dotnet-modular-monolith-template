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

    [HttpDelete("{id}"), Authorize(Roles = "superuser")]
    public async Task<ActionResult> DeleteUser(Guid id) =>
        HandleResult(await _userService.DeleteUser(id));
}
