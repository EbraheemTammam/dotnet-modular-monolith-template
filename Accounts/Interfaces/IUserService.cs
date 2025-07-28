using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Base.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface IUserService
{
    Response<IEnumerable<UserDTO>> GetAllUsers();
    Task<Response<UserDTO>> GetUser(string searchField, string searchValue);
    Task<Response> DeleteUser(Guid id);
    Task<Response<UserDTO>> Register(HttpRequest request, UserAddDTO userCreateDTO, IUrlHelper Url);
    Task<Response> ConfirmEmail(string userId, string token);
    Task<Response> VerifyPhoneNumber(string PhoneNumber, string token);
    Task<Response<UserDTO>> PartialUpdateUser(string userId, UserPartialUpdateDTO userPartialUpdateDTO);
    Task<Response> UpdatePassword(string userId, UserUpdatePasswordDTO userUpdatePasswordDTO);
}
