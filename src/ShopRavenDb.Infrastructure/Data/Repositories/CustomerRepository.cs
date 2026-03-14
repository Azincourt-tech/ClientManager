using Raven.Client.Documents.Session;

namespace ShopRavenDb.Infrastructure.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDocumentStore _documentStore;

        public CustomerRepository(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            await documentSession.StoreAsync(customer).ConfigureAwait(false);
            await documentSession.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteCustomerByIdAsync(Guid id)
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            // Try loading with prefix first (default RavenDB behavior)
            var customer = await documentSession.LoadAsync<Customer>($"Customer/{id}").ConfigureAwait(false);
            if (customer is not null)
            {
                documentSession.Delete(customer);
                await documentSession.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            return await documentSession.LoadAsync<Customer>($"Customer/{id}").ConfigureAwait(false);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            var customers = await documentSession.Query<Customer>().ToListAsync().ConfigureAwait(false);
            return customers;
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            var customerEntity = await documentSession.LoadAsync<Customer>($"Customer/{customer.Id}").ConfigureAwait(false);

            if (customerEntity is not null)
            {
                customerEntity.UpdateDetails(customer.Name, customer.Email, customer.Document, customer.Address);
            }

            await documentSession.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}