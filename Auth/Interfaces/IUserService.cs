using Base.Responses;
using Users.DTOs;
using Users.Interfaces;

namespace Auth.Interfaces;

public interface IExtendedUserService : IUserService
{
    Task<Response<UserDTO>> PartialUpdateUser(UserPartialUpdateDTO userPartialUpdateDTO);
    Task<Response> UpdatePassword(UserUpdatePasswordDTO userUpdatePasswordDTO);
}
