using FluentValidation;
using ShopRavenDb.Domain.Core.Responses;
using ShopRavenDb.Domain.Model;
using ShopRavenDb.Domain.Core.Interfaces.Services;
using ShopRavenDb.Application.Interfaces;
using ShopRavenDb.Application.Dtos;
using AutoMapper;

namespace ShopRavenDb.Application
{
    public class CustomerApplication : ICustomerApplication
    {
        private readonly ICustomerService _customerService;
        private readonly IDocumentService _documentService;
        private readonly IMapper _mapper;
        private readonly FluentValidation.IValidator<CustomerDto> _validator;

        public CustomerApplication(ICustomerService customerService,
                                   IDocumentService documentService,
                                   IMapper mapper,
                                   FluentValidation.IValidator<CustomerDto> validator)
        {
            _customerService = customerService;
            _documentService = documentService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResponse<string>> AddCustomerAsync(CustomerDto customerDto)
        {
            await _validator.ValidateAndThrowAsync(customerDto);
            var customer = _mapper.Map<Customer>(customerDto);
            await _customerService.AddCustomerAsync(customer).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(customer.Id, "Customer inserted successfully!");
        }

        public async Task<ServiceResponse<string>> DeleteCustomerByIdAsync(string id)
        {
            var documentCount = await _documentService.GetDocumentCountByCustomerIdAsync(id).ConfigureAwait(false);
            if (documentCount > 0)
            {
                return ServiceResponse<string>.Fail("Cannot delete customer with saved documents.");
            }

            await _customerService.DeleteCustomerByIdAsync(id).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(id, "Customer deleted successfully!");
        }

        public async Task<ServiceResponse<CustomerDto?>> GetCustomerByIdAsync(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);
            if (customer == null)
                return ServiceResponse<CustomerDto?>.Fail("Customer not found.");
                
            var customerDto = _mapper.Map<CustomerDto>(customer);
            return ServiceResponse<CustomerDto?>.Ok(customerDto);
        }

        public async Task<ServiceResponse<IEnumerable<CustomerDto>>> GetCustomersAsync()
        {
            var customers = await _customerService.GetCustomersAsync().ConfigureAwait(false);
            var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return ServiceResponse<IEnumerable<CustomerDto>>.Ok(customersDto);
        }

        public async Task<ServiceResponse<string>> UpdateCustomerAsync(CustomerDto customerDto)
        {
            await _validator.ValidateAndThrowAsync(customerDto);
            var customer = _mapper.Map<Customer>(customerDto);
            await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(customer.Id, "Customer updated successfully!");
        }
    }
}