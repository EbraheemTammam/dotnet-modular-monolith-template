using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

using Base.Responses;
using Base.Utilities;
using Base.Interfaces;
using Base.Data;
using Users.Services;
using Users.Models;
using Users.DTOs;
using Users.Data;
using Auth.Interfaces;

namespace Auth.Services;

internal class ExtendedUserService : UserService, IExtendedUserService
{
    private readonly ICurrentLoggedInUser _currentUser;
    public ExtendedUserService(
        UserManager<User> userManager,
        IHostEnvironment env,
        HttpContextAccessor httpContextAccessor,
        ICurrentLoggedInUser currentUser,
        VerificationRepository verificationObjects,
        BaseDbContext baseDbContext,
        INotificationService notificationService
    )
        : base(userManager, env, httpContextAccessor, verificationObjects, baseDbContext, notificationService)
    {
        _currentUser = currentUser;
    }

    public async Task<Response<UserDTO>> PartialUpdateUser(UserPartialUpdateDTO userPartialUpdateDTO)
    {
        User? user = await _currentUser.GetUser();
        user.FirstName = userPartialUpdateDTO.FirstName ?? user.FirstName;
        user.LastName = userPartialUpdateDTO.LastName ?? user.LastName;
        await _userManager.UpdateAsync(user);
        if (userPartialUpdateDTO.ProfilePicture is not null)
            await userPartialUpdateDTO.ProfilePicture!.SaveAsWebP(user.Id.ToString(), $"{_env.ContentRootPath}/{user.ProfilePicture!.SaveTo}");
        return Response<UserDTO>.Success(UserDTO.FromModel(user, _request));
    }

    public async Task<Response> UpdatePassword(UserUpdatePasswordDTO userUpdatePasswordDTO)
    {
        User? user = await _currentUser.GetUser();
        await _userManager.ChangePasswordAsync(user, userUpdatePasswordDTO.CurrentPassword, userUpdatePasswordDTO.NewPassword);
        return Response.Success();
    }
}
