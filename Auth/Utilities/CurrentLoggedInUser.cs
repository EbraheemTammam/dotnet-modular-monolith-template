using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

using Users.Models;
using Auth.Interfaces;

namespace Auth.Utilities;

public class CurrentLoggedInUser : ICurrentLoggedInUser
{
    private readonly UserManager<User> _userManager;
    public CurrentLoggedInUser(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        UserId = httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        _userManager = userManager;
    }
    public string UserId { get; }
    public async Task<User> GetUser() =>
        (await _userManager.FindByIdAsync(UserId))!;
}
