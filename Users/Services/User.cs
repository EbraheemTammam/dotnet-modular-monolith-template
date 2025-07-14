using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Base.Responses;
using Users.DTOs;
using Users.Interfaces;
using Users.Models;

namespace Users.Services;

public class UserService : IUserService
{
    protected readonly HttpRequest _request;
    protected readonly UserManager<User> _userManager;
    public UserService(UserManager<User> userManager, HttpContextAccessor httpContextAccessor)
    {
        _request = httpContextAccessor.HttpContext.Request;
        _userManager = userManager;
    }

    public Response<IEnumerable<UserDTO>> GetAllUsers() =>
        Response<IEnumerable<UserDTO>>.Success(
            _userManager.Users.Select(u => UserDTO.FromModel(u, _request))
        );

    public async Task<Response<UserDTO>> GetUser(Guid id)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return Response<UserDTO>.NotFound(id.ToString(), nameof(User));
        return Response<UserDTO>.Success(UserDTO.FromModel(user, _request));
    }

    public async Task<Response> DeleteUser(Guid id)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return Response<UserDTO>.NotFound(id.ToString(), nameof(User));
        await _userManager.DeleteAsync(user);
        return Response.Success();
    }
}
