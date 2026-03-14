using FluentValidation;

namespace ShopRavenDb.Application
{
    public class CustomerApplication : ICustomerApplication
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly FluentValidation.IValidator<CustomerDto> _validator;

        public CustomerApplication(ICustomerService customerService,
                                   IMapper mapper,
                                   FluentValidation.IValidator<CustomerDto> validator)
        {
            _customerService = customerService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task AddCustomerAsync(CustomerDto customerDto)
        {
            await _validator.ValidateAndThrowAsync(customerDto);
            var customer = _mapper.Map<Customer>(customerDto);
            await _customerService.AddCustomerAsync(customer).ConfigureAwait(false);
        }

        public async Task DeleteCustomerByIdAsync(string id)
        {
            await _customerService.DeleteCustomerByIdAsync(id).ConfigureAwait(false);
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);
            var customerDto = _mapper.Map<CustomerDto>(customer);
            return customerDto;
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
        {
            var customers = await _customerService.GetCustomersAsync().ConfigureAwait(false);
            var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return customersDto;
        }

        public async Task UpdateCustomerAsync(CustomerDto customerDto)
        {
            await _validator.ValidateAndThrowAsync(customerDto);
            var customer = _mapper.Map<Customer>(customerDto);
            await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);
        }
    }
}