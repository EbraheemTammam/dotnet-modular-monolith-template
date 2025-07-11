using Base.Responses;
using Users.DTOs;

namespace Users.Interfaces;

public interface IUserService
{
    Response<IEnumerable<UserDTO>> GetAllUsers();
    Task<Response<UserDTO>> GetUser(Guid id);
    Task<Response<UserDTO>> AddUser(UserAddDTO userCreateDTO);
    Task<Response> DeleteUser(Guid id);
}
