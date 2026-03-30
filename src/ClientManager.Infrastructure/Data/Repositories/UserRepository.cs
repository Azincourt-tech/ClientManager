using Raven.Client.Documents.Session;

namespace ClientManager.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IAsyncDocumentSession _session;

    public UserRepository(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task AddUserAsync(User user)
    {
        await _session.StoreAsync(user, user.Id.ToString()).ConfigureAwait(false);
        await _session.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await _session.LoadAsync<User>(user.Id.ToString()).ConfigureAwait(false);
        if (existingUser is not null)
        {
            existingUser.UpdateDetails(user.Username, user.Email, user.Role);
            await _session.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _session.LoadAsync<User>(id.ToString()).ConfigureAwait(false);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _session.Query<User>()
            .FirstOrDefaultAsync(u => u.Username == username).ConfigureAwait(false);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _session.Query<User>()
            .FirstOrDefaultAsync(u => u.Email == email).ConfigureAwait(false);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        return await _session.Query<User>()
            .Where(u => u.IsActive)
            .ToListAsync().ConfigureAwait(false);
    }
}
