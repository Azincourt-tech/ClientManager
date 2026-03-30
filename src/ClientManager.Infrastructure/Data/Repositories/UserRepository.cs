using Raven.Client.Documents.Session;

namespace ClientManager.Infrastructure.Data.Repositories
{
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

        public async Task DeleteUserByIdAsync(Guid id)
        {
            var user = await _session.LoadAsync<User>(id.ToString()).ConfigureAwait(false);
            if (user is not null)
            {
                user.Deactivate();
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var user = await _session.LoadAsync<User>(id.ToString()).ConfigureAwait(false);
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var user = await _session.Query<User>()
                .FirstOrDefaultAsync(u => u.Username == username)
                .ConfigureAwait(false);
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _session.Query<User>()
                .FirstOrDefaultAsync(u => u.Email == email)
                .ConfigureAwait(false);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _session.Query<User>()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task UpdateUserAsync(User user)
        {
            var userEntity = await _session.LoadAsync<User>(user.Id.ToString()).ConfigureAwait(false);

            if (userEntity is not null)
            {
                userEntity.UpdateDetails(user.Username, user.Email, user.Role);
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
