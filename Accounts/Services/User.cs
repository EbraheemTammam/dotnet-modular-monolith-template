using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using Base.Responses;
using Base.Data;
using Base.Interfaces;
using Accounts.DTOs;
using Accounts.Interfaces;
using Accounts.Models;
using Accounts.Data;

namespace Accounts.Services;

public class UserService : IUserService
{
    private readonly AccountsDbContext _db;
    protected readonly UserManager<User> _userManager;
    private readonly INotificationService _notificationService;
    private readonly VerificationManager _verifications;
    public UserService(
        AccountsDbContext db,
        UserManager<User> userManager,
        VerificationManager verifications,
        INotificationService notificationService
    )
    {
        _db = db;
        _userManager = userManager;
        _notificationService = notificationService;
        _verifications = verifications;
    }

    public Response<IEnumerable<UserDTO>> GetAllUsers() =>
        Response<IEnumerable<UserDTO>>.Success(
            _db.Users.Select(u => UserDTO.FromModel(u))
        );

    public async Task<Response<UserDTO>> GetUser(string searchField, string searchValue)
    {
        searchField = searchField.ToLower();
        UserDTO? user = await _db.Users.Where(
                                           searchField switch
                                           {
                                               "id" => u => u.Id == new Guid(searchValue),
                                               "email" => u => u.Email == searchValue,
                                               "phone_number" => u => u.PhoneNumber == searchValue,
                                               _ => u => false
                                           }
                                       )
                                       .Select(u => UserDTO.FromModel(u))
                                       .FirstOrDefaultAsync();
        return user switch
        {
            null => Response<UserDTO>.NotFound(searchValue, nameof(User), searchField),
            _ => Response<UserDTO>.Success(user)
        };
    }

    public async Task<Response> DeleteUser(Guid id)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return Response<UserDTO>.NotFound(id.ToString(), nameof(User));
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return Response.Success();
    }

    public async Task<Response<UserDTO>> Register(HttpRequest request, UserAddDTO userAddDTO, IUrlHelper Url)
    {
        User user = userAddDTO.ToModel();
        var res = await _userManager.CreateAsync(user, userAddDTO.Password);
        if (!res.Succeeded) return Response<UserDTO>.Fail(res.Errors.First().Description);
        await _userManager.AddToRoleAsync(user, "user");

        await user.SendEmailConfirmation(_userManager, request.Scheme, _notificationService, Url);
        await user.SendPhoneNumberConfirmation(_notificationService, _verifications);

        return Response<UserDTO>.Success(UserDTO.FromModel(user));
    }

    public async Task<Response> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response.NotFound(userId, nameof(User));

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return Response.Fail(result.Errors.First().Description);

        return Response.Success();
    }

    public async Task<Response> VerifyPhoneNumber(string UserId, string token)
    {
        User user = (await _userManager.FindByIdAsync(UserId))!;
        Verification? verification = await _verifications.GetAsync(user.PhoneNumber!);
        if (user is null || verification is null || verification.Token != token)
            return Response.Fail("Invalid verification code");

        user.PhoneNumberConfirmed = true;
        await _userManager.UpdateAsync(user);

        return Response.Success();
    }

    public async Task<Response<UserDTO>> PartialUpdateUser(string userId, UserPartialUpdateDTO userPartialUpdateDTO)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response<UserDTO>.NotFound(userId, nameof(User));
        user.FirstName = userPartialUpdateDTO.FirstName ?? user.FirstName;
        user.LastName = userPartialUpdateDTO.LastName ?? user.LastName;
        await _userManager.UpdateAsync(user);
        return Response<UserDTO>.Success(UserDTO.FromModel(user));
    }

    public async Task<Response> UpdatePassword(string userId, UserUpdatePasswordDTO userUpdatePasswordDTO)
    {
        User? user = await _userManager.FindByIdAsync(userId);
        if (user is null) return Response.NotFound(userId, nameof(User));
        await _userManager.ChangePasswordAsync(user, userUpdatePasswordDTO.CurrentPassword, userUpdatePasswordDTO.NewPassword);
        return Response.Success();
    }
}
