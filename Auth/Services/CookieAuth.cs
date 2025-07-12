using Microsoft.AspNetCore.Identity;

using Base.Responses;
using Users.Models;
using Auth.DTOs;
using Auth.Interfaces;

namespace Auth.Services;

internal class CookieAuthService : ICookieAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public CookieAuthService(UserManager<User> manager, SignInManager<User> signInManager)
    {
        _userManager = manager;
        _signInManager = signInManager;
    }

    public async Task<Response> LoginAsync(LoginDTO loginDTO)
    {
        User? user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if(user is not null)
        {
            var result = await _signInManager.PasswordSignInAsync(user, loginDTO.Password, false, false);
            if(result.Succeeded) return Response.Success();
        }
        return Response.UnAuthorized;
    }

    public async Task<Response> LogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return Response.Success();
    }
}
