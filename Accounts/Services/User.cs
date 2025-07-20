using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

using Base.Responses;
using Base.Models;
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
    private readonly VerificationRepository _verificationObjects;
    private readonly BaseDbContext _baseDbContext;
    public UserService(
        UserManager<User> userManager,
        IHostEnvironment env,
        VerificationRepository verificationObjects,
        BaseDbContext baseDbContext,
        INotificationService notificationService
    )
    {
        _userManager = userManager;
        _env = env;
        _notificationService = notificationService;
        _verificationObjects = verificationObjects;
        _baseDbContext = baseDbContext;
    }

    public Response<IEnumerable<UserDTO>> GetAllUsers(HttpRequest request) =>
        Response<IEnumerable<UserDTO>>.Success(
            _userManager.Users.Select(u => UserDTO.FromModel(u, request))
        );

    public async Task<Response<UserDTO>> GetUser(HttpRequest request, string searchField, string searchValue)
    {
        searchField = searchField.ToLower();
        User? user = (
            searchField == "id" ? await _userManager.FindByIdAsync(searchValue) :
            searchField == "email" ? await _userManager.FindByEmailAsync(searchValue) :
            searchField == "phone_number" ? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == searchValue) :
            null
        );
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
        await _userManager.AddToRoleAsync(user, "user");

        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string? confirmationLink = Url.Action(
            nameof(ConfirmEmail),
            "Auth",
            new { userId = user.Id, token },
            request.Scheme
        );

        string emailBody = $"Please confirm your email by clicking <a href='{HtmlEncoder.Default.Encode(confirmationLink!)}'>here</a>.";
        await _notificationService.SendEmail(userAddDTO.Email, "Confirm Your Email", emailBody);

        var code = new Random().Next(100000, 999999).ToString();
        var verification = new Verification
        {
            PhoneNumber = userAddDTO.PhoneNumber,
            Token = code
        };
        await _verificationObjects.AddVerificationAsync(verification);
        await _notificationService.SendSms(userAddDTO.PhoneNumber, $"Your verification code is {code}");

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
        Verification? verification = await _verificationObjects.GetVerificationAsync(phoneNumber);
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
            await userPartialUpdateDTO.ProfilePicture!.SaveAsWebP(user.Id.ToString(), $"{_env.ContentRootPath}/{user.ProfilePicture!.SaveTo}");
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
