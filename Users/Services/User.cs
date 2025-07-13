using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using Base.Interfaces;
using Base.Responses;
using Base.Utilities;
using Base.Models;
using Base.Data;
using Users.DTOs;
using Users.Interfaces;
using Users.Models;
using Users.Data;

namespace Users.Services;

public class UserService : IUserService
{
    protected readonly HttpRequest _request;
    protected readonly IHostEnvironment _env;
    protected readonly UserManager<User> _userManager;
    protected readonly VerificationRepository _verificationObjects;
    protected readonly BaseDbContext _baseDbContext;
    protected readonly INotificationService _notificationService;
    public UserService(
        UserManager<User> userManager,
        IHostEnvironment env,
        HttpContextAccessor httpContextAccessor,
        VerificationRepository verificationObjects,
        BaseDbContext baseDbContext,
        INotificationService notificationService
    )
    {
        _request = httpContextAccessor.HttpContext.Request;
        _env = env;
        _userManager = userManager;
        _verificationObjects = verificationObjects;
        _baseDbContext = baseDbContext;
        _notificationService = notificationService;
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

    public async Task<Response<UserDTO>> AddUser(UserAddDTO userAddDTO)
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

    public async Task<Response> DeleteUser(Guid id)
    {
        User? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
            return Response<UserDTO>.NotFound(id.ToString(), nameof(User));
        await _userManager.DeleteAsync(user);
        return Response.Success();
    }
}
