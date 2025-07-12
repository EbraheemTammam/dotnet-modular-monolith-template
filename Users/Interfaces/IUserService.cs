using Base.Responses;
using Users.DTOs;

namespace Users.Interfaces;

public interface IUserService
{
    Response<IEnumerable<UserDTO>> GetAllUsers();
    Task<Response<UserDTO>> GetUser(Guid id);
    Task<Response<UserDTO>> AddUser(UserAddDTO userCreateDTO);
    Task<Response> SendEmailVerification(string email);
    Task<Response> SendPhoneNumberVerification(string phoneNumber);
    Task<Response> VerifyEmail(string token);
    Task<Response> VerifyPhoneNumber(string token);
    Task<Response> DeleteUser(Guid id);
}
