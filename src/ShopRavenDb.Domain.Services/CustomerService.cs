using ShopRavenDb.Domain.Core.Interfaces.Validators;

namespace ShopRavenDb.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailValidator _emailValidator;

        public CustomerService(ICustomerRepository customerRepository,
                               IEmailValidator emailValidator)
        {
            _customerRepository = customerRepository;
            _emailValidator = emailValidator;

        }

        public async Task AddCustomerAsync(Customer customer)
        {
            if (!_emailValidator.IsValid(customer.Email))
            {
                throw new Exception("Invalid email");
            }
            customer.Activate();
            await _customerRepository.AddCustomerAsync(customer).ConfigureAwait(false);
        }

        public async Task DeleteCustomerByIdAsync(string id)
        {
            await _customerRepository.DeleteCustomerByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<Customer?> GetCustomerByIdAsync(string id)
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