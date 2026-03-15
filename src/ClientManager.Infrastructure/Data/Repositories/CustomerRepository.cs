using Raven.Client.Documents.Session;

namespace ClientManager.Infrastructure.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IAsyncDocumentSession _session;

        public CustomerRepository(IAsyncDocumentSession session)
        {
            _session = session;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _session.StoreAsync(customer, customer.Id.ToString()).ConfigureAwait(false);
            await _session.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteCustomerByIdAsync(Guid id)
        {
            var customer = await _session.LoadAsync<Customer>(id.ToString()).ConfigureAwait(false);
            if (customer is not null)
            {
                customer.Delete();
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _session.LoadAsync<Customer>(id.ToString()).ConfigureAwait(false);
            return customer is not null && !customer.IsDeleted ? customer : null;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _session.Query<Customer>()
                                .Where(x => !x.IsDeleted)
                                .ToListAsync().ConfigureAwait(false);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var customerEntity = await _session.LoadAsync<Customer>(customer.Id.ToString()).ConfigureAwait(false);

            if (customerEntity is not null)
            {
                customerEntity.UpdateDetails(customer.Name, customer.Email, customer.Document, customer.Address);
                await _session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Obtém o histórico de revisões (rastreabilidade) do cliente.
        /// Requer que as Revisions estejam ativadas no RavenDB Studio.
        /// </summary>
        public async Task<IEnumerable<Customer>> GetCustomerHistoryAsync(Guid id)
        {
            return await _session.Advanced.Revisions.GetForAsync<Customer>(id.ToString()).ConfigureAwait(false);
        }
    }
}
