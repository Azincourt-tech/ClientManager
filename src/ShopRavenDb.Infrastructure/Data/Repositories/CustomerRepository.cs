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
            // Passamos o ID explicitamente como string para o RavenDB
            await documentSession.StoreAsync(customer, customer.Id.ToString()).ConfigureAwait(false);
            await documentSession.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteCustomerByIdAsync(Guid id)
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            // Buscamos pelo ID exato (Guid como string)
            var customer = await documentSession.LoadAsync<Customer>(id.ToString()).ConfigureAwait(false);
            if (customer is not null)
            {
                documentSession.Delete(customer);
                await documentSession.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            using IAsyncDocumentSession documentSession = _documentStore.OpenAsyncSession();
            // Carregamos usando o Guid como string
            return await documentSession.LoadAsync<Customer>(id.ToString()).ConfigureAwait(false);
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
            // Carregamos usando o Guid como string
            var customerEntity = await documentSession.LoadAsync<Customer>(customer.Id.ToString()).ConfigureAwait(false);

            if (customerEntity is not null)
            {
                customerEntity.UpdateDetails(customer.Name, customer.Email, customer.Document, customer.Address);
            }

            await documentSession.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}