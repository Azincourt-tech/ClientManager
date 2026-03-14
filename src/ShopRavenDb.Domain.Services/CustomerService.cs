using ShopRavenDb.Domain.Core.Interfaces.Validators;

namespace ShopRavenDb.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            customer.Activate();
            await _customerRepository.AddCustomerAsync(customer).ConfigureAwait(false);
        }

        public async Task DeleteCustomerByIdAsync(Guid id)
        {
            await _customerRepository.DeleteCustomerByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<Customer?> GetCustomerByIdAsync(Guid id)
        {
            return await _customerRepository.GetCustomerByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _customerRepository.GetCustomersAsync().ConfigureAwait(false);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerRepository.UpdateCustomerAsync(customer).ConfigureAwait(false);
        }
    }
}