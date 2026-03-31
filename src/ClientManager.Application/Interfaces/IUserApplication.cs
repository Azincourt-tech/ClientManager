using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Responses;

namespace ClientManager.Application.Interfaces
{
    public interface IUserApplication
    {
        Task<ServiceResponse<Guid>> AddUserAsync(CreateUserDto userDto);

        Task<ServiceResponse<string>> UpdateUserAsync(Guid id, CreateUserDto userDto);

        Task<ServiceResponse<string>> DeleteUserByIdAsync(Guid id);

        Task<ServiceResponse<IEnumerable<UserDto>>> GetUsersAsync();

        Task<ServiceResponse<UserDto?>> GetUserByIdAsync(Guid id);
    }
}
