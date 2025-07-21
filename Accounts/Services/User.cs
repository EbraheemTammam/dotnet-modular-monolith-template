using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using Base.Responses;
using Base.Data;
using Base.Interfaces;
using Base.Utilities;
using Accounts.DTOs;
using Accounts.Interfaces;
using Accounts.Models;
using Accounts.Data;

namespace Accounts.Services;

public class UserService : IUserService
{
    protected readonly UserManager<User> _userManager;
    private readonly IHostEnvironment _env;
    private readonly INotificationService _notificationService;
    private readonly VerificationManager _verifications;
    private readonly BaseDbContext _baseDbContext;
    public UserService(
        UserManager<User> userManager,
        IHostEnvironment env,
        VerificationManager verifications,
        BaseDbContext baseDbContext,
        INotificationService notificationService
    )
    {
        _userManager = userManager;
        _env = env;
        _notificationService = notificationService;
        _verifications = verifications;
        _baseDbContext = baseDbContext;
    }

    public Response<IEnumerable<UserDTO>> GetAllUsers(HttpRequest request) =>
        Response<IEnumerable<UserDTO>>.Success(
            _userManager.Users.Select(u => UserDTO.FromModel(u, request))
        );

    public async Task<Response<UserDTO>> GetUser(HttpRequest request, string searchField, string searchValue)
    {
        searchField = searchField.ToLower();
        User? user = searchField switch
        {
            "id" => await _userManager.FindByIdAsync(searchValue),
            "email" => await _userManager.FindByEmailAsync(searchValue),
            "phone_number" => await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == searchValue),
            _ => null
        };
        return user switch
        {
            null => Response<UserDTO>.NotFound(searchValue, nameof(User), searchField),
            _ => Response<UserDTO>.Success(UserDTO.FromModel(user, request))
        };
    }

    public async Task<Response> DeleteUser(Guid id)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return Response<UserDTO>.NotFound(id.ToString(), nameof(User));
        await _userManager.DeleteAsync(user);
        return Response.Success();
    }

    public async Task<Response<UserDTO>> Register(HttpRequest request, UserAddDTO userAddDTO, IUrlHelper Url)
    {
        User user = userAddDTO.ToModel();
        var res = await _userManager.CreateAsync(user, userAddDTO.Password);
        if (!res.Succeeded) return Response<UserDTO>.Fail(res.Errors.First().Description);
        await _userManager.AddToRoleAsync(user, "user");

        if (userAddDTO.ProfilePicture is not null)
            await user.SaveProfilePicture(userAddDTO.ProfilePicture, _env.ContentRootPath);

        await user.SendEmailConfirmation(_userManager, request.Scheme, _notificationService, Url);
        await user.SendPhoneNumberConfirmation(_notificationService, _verifications);

        return Response<UserDTO>.Success(UserDTO.FromModel(user, request));
    }

    public async Task<Response> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response.NotFound(userId, nameof(User));

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded switch
        {
            true => Response.Success(),
            false => Response.Fail(result.Errors.First().Description)
        };
    }

    public async Task<Response> VerifyPhoneNumber(string phoneNumber, string token)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        Verification? verification = await _verifications.GetAsync(phoneNumber);
        if (user is null || verification is null || verification.Token != token)
            return Response.Fail("Invalid verification code");
        user.PhoneNumberConfirmed = true;
        await _userManager.UpdateAsync(user);
        return Response.Success();
    }

    public async Task<Response<UserDTO>> PartialUpdateUser(HttpRequest request, string userId, UserPartialUpdateDTO userPartialUpdateDTO)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response<UserDTO>.NotFound(userId, nameof(User));
        user.FirstName = userPartialUpdateDTO.FirstName ?? user.FirstName;
        user.LastName = userPartialUpdateDTO.LastName ?? user.LastName;
        await _userManager.UpdateAsync(user);
        if (userPartialUpdateDTO.ProfilePicture is not null)
            await user.SaveProfilePicture(userPartialUpdateDTO.ProfilePicture, _env.ContentRootPath);
        return Response<UserDTO>.Success(UserDTO.FromModel(user, request));
    }

    public async Task<Response> UpdatePassword(string userId, UserUpdatePasswordDTO userUpdatePasswordDTO)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response.NotFound(userId, nameof(User));
        await _userManager.ChangePasswordAsync(user, userUpdatePasswordDTO.CurrentPassword, userUpdatePasswordDTO.NewPassword);
        return Response.Success();
    }
}
