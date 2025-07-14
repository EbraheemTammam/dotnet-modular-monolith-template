using Base.Responses;
using Users.DTOs;
using Users.Interfaces;

namespace Auth.Interfaces;

public interface IExtendedUserService : IUserService
{
    Task<Response<UserDTO>> Register(UserAddDTO userCreateDTO);
    Task<Response> SendEmailVerification(string email);
    Task<Response> SendPhoneNumberVerification(string phoneNumber);
    Task<Response> VerifyEmail(string email, string token);
    Task<Response> VerifyPhoneNumber(string phoneNumber, string token);
    Task<Response<UserDTO>> PartialUpdateUser(UserPartialUpdateDTO userPartialUpdateDTO);
    Task<Response> UpdatePassword(UserUpdatePasswordDTO userUpdatePasswordDTO);
}
