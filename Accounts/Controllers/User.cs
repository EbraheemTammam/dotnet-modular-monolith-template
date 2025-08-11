using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Base.Controllers;
using Accounts.DTOs;
using Accounts.Interfaces;

namespace Accounts.Controllers;

public class AccountsController : ApiBaseController
{
    private readonly IUserService _userService;

    public AccountsController(IUserService userService) =>
        _userService = userService;

    [HttpGet, Authorize(Roles = "superuser")]
    public ActionResult<IEnumerable<UserDTO>> GetAllUsers() =>
        HandleResult(_userService.GetAllUsers());

    [HttpGet("search"), Authorize(Roles = "superuser")]
    public async Task<ActionResult<UserDTO>> GetUser([FromQuery, Required] string searchField, [FromQuery, Required] string searchValue) =>
        HandleResult(await _userService.GetUser(searchField, searchValue));

    [HttpDelete("{id}"), Authorize(Roles = "superuser")]
    public async Task<ActionResult> DeleteUser(Guid id) =>
        HandleResult(await _userService.DeleteUser(id));

    [HttpGet("profile"), Authorize]
    public async Task<ActionResult<UserDTO>> GetProfile() =>
        HandleResult(await _userService.GetUser("id", User.Identity!.Name!));

    [HttpPatch("update_profile"), Authorize]
    public async Task<ActionResult<UserDTO>> PartialUpdateUser([FromForm] UserPartialUpdateDTO userPartialUpdateDTO) =>
        HandleResult(await _userService.PartialUpdateUser(User.Identity!.Name!, userPartialUpdateDTO));
}
