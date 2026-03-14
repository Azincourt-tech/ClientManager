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
        private readonly IValidator<CustomerDto> _validator;

        public CustomerApplication(ICustomerService customerService, IDocumentService documentService, IMapper mapper, IValidator<CustomerDto> validator)
        {
            _customerService = customerService;
            _documentService = documentService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ServiceResponse<Guid>> AddCustomerAsync(CustomerDto customerDto)
        {
            await _validator.ValidateAndThrowAsync(customerDto);
            var customer = _mapper.Map<Customer>(customerDto);
            await _customerService.AddCustomerAsync(customer).ConfigureAwait(false);
            return ServiceResponse<Guid>.Ok(customer.Id, "CustomerInserted");
        }

        public async Task<ServiceResponse<string>> DeleteCustomerByIdAsync(Guid id)
        {
            var documentCount = await _documentService.GetDocumentCountByCustomerIdAsync(id).ConfigureAwait(false);
            if (documentCount > 0)
            {
                return ServiceResponse<string>.Fail("DeleteCustomerWithDocsError");
            }

            await _customerService.DeleteCustomerByIdAsync(id).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(id.ToString(), "CustomerDeleted");
        }

        public async Task<ServiceResponse<CustomerDto?>> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);
            if (customer == null)
                return ServiceResponse<CustomerDto?>.Fail("CustomerNotFound");
            
            // Re-evaluate status based on current documents
            var documents = await _documentService.GetDocumentsByCustomerIdAsync(id).ConfigureAwait(false);
            customer.EvaluateVerificationStatus(documents);
            await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);

            var customerDto = _mapper.Map<CustomerDto>(customer);
            return ServiceResponse<CustomerDto?>.Ok(customerDto);
        }

        public async Task<ServiceResponse<IEnumerable<CustomerDto>>> GetCustomersAsync()
        {
            var customers = await _customerService.GetCustomersAsync().ConfigureAwait(false);
            
            // Re-evaluate statuses for all customers (optional optimization: only if needed)
            foreach (var customer in customers)
            {
                var documents = await _documentService.GetDocumentsByCustomerIdAsync(customer.Id).ConfigureAwait(false);
                customer.EvaluateVerificationStatus(documents);
                await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);
            }

            var customersDto = _mapper.Map<IEnumerable<CustomerDto>>(customers);
            return ServiceResponse<IEnumerable<CustomerDto>>.Ok(customersDto);
        }

        public async Task<ServiceResponse<string>> UpdateCustomerAsync(CustomerDto customerDto)
        {
            if (string.IsNullOrEmpty(customerDto.Id) || !Guid.TryParse(customerDto.Id, out Guid customerGuid))
                return ServiceResponse<string>.Fail("CustomerNotFound");

            await _validator.ValidateAndThrowAsync(customerDto);
            
            var existingCustomer = await _customerService.GetCustomerByIdAsync(customerGuid).ConfigureAwait(false);
            if (existingCustomer == null)
                return ServiceResponse<string>.Fail("CustomerNotFound");

            // Update details (Name, Email, Document, Address) via domain method
            var address = customerDto.Address != null ? _mapper.Map<Address>(customerDto.Address) : null;
            existingCustomer.UpdateDetails(customerDto.Name, customerDto.Email, customerDto.Document, address);
            
            // Re-evaluate status as types or documents might have changed (though documents didn't change here, Type might have)
            var documents = await _documentService.GetDocumentsByCustomerIdAsync(customerGuid).ConfigureAwait(false);
            existingCustomer.EvaluateVerificationStatus(documents);

            await _customerService.UpdateCustomerAsync(existingCustomer).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(existingCustomer.Id.ToString(), "CustomerUpdated");
        }

        public async Task<ServiceResponse<string>> VerifyCustomerAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);
            if (customer == null) return ServiceResponse<string>.Fail("CustomerNotFound");

            var documents = await _documentService.GetDocumentsByCustomerIdAsync(id).ConfigureAwait(false);
            customer.EvaluateVerificationStatus(documents);
            await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);

            return ServiceResponse<string>.Ok(customer.Status.ToString(), "CustomerStatusEvaluated");
        }
    }
}