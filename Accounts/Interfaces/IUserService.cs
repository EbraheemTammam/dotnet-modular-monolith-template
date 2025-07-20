using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Base.Responses;
using Accounts.DTOs;

namespace Accounts.Interfaces;

public interface IUserService
{
    Response<IEnumerable<UserDTO>> GetAllUsers(HttpRequest request);
    Task<Response<UserDTO>> GetUser(HttpRequest request, string searchField, string searchValue);
    Task<Response> DeleteUser(Guid id);
    Task<Response<UserDTO>> Register(HttpRequest request, UserAddDTO userCreateDTO, IUrlHelper Url);
    Task<Response> ConfirmEmail(string userId, string token);
    Task<Response> VerifyPhoneNumber(string phoneNumber, string token);
    Task<Response<UserDTO>> PartialUpdateUser(HttpRequest request, string userId, UserPartialUpdateDTO userPartialUpdateDTO);
    Task<Response> UpdatePassword(string userId, UserUpdatePasswordDTO userUpdatePasswordDTO);
}
