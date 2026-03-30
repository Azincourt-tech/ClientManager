namespace ClientManager.Domain.Core.Interfaces.Services;

public interface IUserService
{
    Task AddUserAsync(User user);

    Task UpdateUserAsync(User user);

    Task<User?> GetUserByIdAsync(Guid id);

    Task<User?> GetUserByUsernameAsync(string username);

    Task<User?> GetUserByEmailAsync(string email);

    Task<IEnumerable<User>> GetUsersAsync();
}
