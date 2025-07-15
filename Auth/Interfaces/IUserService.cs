using Microsoft.AspNetCore.Mvc;

using Base.Responses;
using Users.DTOs;
using Users.Interfaces;

namespace Auth.Interfaces;

public interface IExtendedUserService : IUserService
{
    Task<Response<UserDTO>> Register(UserAddDTO userCreateDTO, IUrlHelper Url);
    Task<Response> ConfirmEmail(string userId, string token);
    Task<Response> VerifyPhoneNumber(string phoneNumber, string token);
    Task<Response<UserDTO>> PartialUpdateUser(UserPartialUpdateDTO userPartialUpdateDTO);
    Task<Response> UpdatePassword(UserUpdatePasswordDTO userUpdatePasswordDTO);
}
