using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Responses;

namespace ClientManager.Application.Interfaces;

public interface IUserApplication
{
    Task<ServiceResponse<UserDto>> CreateUserAsync(CreateUserDto createUserDto);

    Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid id);

    Task<ServiceResponse<IEnumerable<UserDto>>> GetUsersAsync();

    Task<ServiceResponse<UserDto>> UpdateUserAsync(Guid id, CreateUserDto updateUserDto);

    Task<ServiceResponse<string>> DeleteUserAsync(Guid id);
}
