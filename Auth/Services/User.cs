using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using Base.Responses;
using Base.Utilities;
using Base.Interfaces;
using Base.Data;
using Base.Models;
using Users.Services;
using Users.Models;
using Users.DTOs;
using Users.Data;
using Auth.Interfaces;

namespace Auth.Services;

internal class ExtendedUserService : UserService, IExtendedUserService
{
    private readonly ICurrentLoggedInUser _currentUser;
    private readonly IHostEnvironment _env;
    private readonly INotificationService _notificationService;
    private readonly VerificationRepository _verificationObjects;
    private readonly BaseDbContext _baseDbContext;

    public ExtendedUserService(
        UserManager<User> userManager,
        IHostEnvironment env,
        HttpContextAccessor httpContextAccessor,
        ICurrentLoggedInUser currentUser,
        VerificationRepository verificationObjects,
        BaseDbContext baseDbContext,
        INotificationService notificationService
    )
        : base(userManager, httpContextAccessor)
    {
        _currentUser = currentUser;
        _env = env;
        _notificationService = notificationService;
        _verificationObjects = verificationObjects;
        _baseDbContext = baseDbContext;
    }

    public async Task<Response<UserDTO>> Register(UserAddDTO userAddDTO)
    {
        User user = userAddDTO.ToModel();
        if (userAddDTO.ProfilePicture is not null)
        {
            Document profilePicture = new Document
            {
                FileName = $"{user.Id}.webp",
                SaveTo = $"/users/profile_pictures",
                Domain = _env.ContentRootPath
            };
            _baseDbContext.Documents.Add(profilePicture);
            await _baseDbContext.SaveChangesAsync();
            await userAddDTO.ProfilePicture.SaveAsWebP(user.Id.ToString(), $"{_env.ContentRootPath}/{user.ProfilePicture!.SaveTo}");
            user.ProfilePictureId = profilePicture.Id;
        }
        await _userManager.CreateAsync(user, userAddDTO.Password);
        return Response<UserDTO>.Success(UserDTO.FromModel(user, _request));
    }

    public Task<Response> SendEmailVerification(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<Response> SendPhoneNumberVerification(string phoneNumber)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        if (user is not null)
        {
            var code = new Random().Next(100000, 999999).ToString();
            var verification = new Verification
            {
                PhoneNumber = phoneNumber,
                Token = code
            };
            await _verificationObjects.AddVerificationAsync(verification);
            await _notificationService.SendSms(phoneNumber, $"Your verification code is {code}");
        }
        return Response.Success();
    }

    public Task<Response> VerifyEmail(string email, string token)
    {
        throw new NotImplementedException();
    }

    public async Task<Response> VerifyPhoneNumber(string phoneNumber, string token)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        Verification? verification = await _verificationObjects.GetVerificationAsync(phoneNumber);
        if (user is null || verification is null || verification.Token != token)
            return Response.Fail("Invalid verification code");
        user.PhoneNumberConfirmed = true;
        await _userManager.UpdateAsync(user);
        return Response.Success();
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
