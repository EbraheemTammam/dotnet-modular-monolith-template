using Microsoft.AspNetCore.Identity;

using Shared.Responses;
using Accounts.Models;
using Accounts.DTOs;
using Accounts.Interfaces;

namespace Accounts.Services;

internal class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> manager, SignInManager<User> signInManager)
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
