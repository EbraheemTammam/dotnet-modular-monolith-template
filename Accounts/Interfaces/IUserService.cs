using Microsoft.AspNetCore.Mvc;

using Base.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface IUserService
{
    Response<IEnumerable<UserDTO>> GetAllUsers();
    Task<Response<UserDTO>> GetUser(string searchField, string searchValue);
    Task<Response> DeleteUser(Guid id);
    Task<Response<UserDTO>> Register(UserAddDTO userCreateDTO, IUrlHelper Url);
    Task<Response> ConfirmEmail(string userId, string token);
    Task<Response> VerifyPhoneNumber(string phoneNumber, string token);
    Task<Response<UserDTO>> PartialUpdateUser(UserPartialUpdateDTO userPartialUpdateDTO);
    Task<Response> UpdatePassword(UserUpdatePasswordDTO userUpdatePasswordDTO);
}
