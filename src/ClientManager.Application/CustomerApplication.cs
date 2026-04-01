using ClientManager.Application.Mappers;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Core.Responses;
using FluentValidation;

namespace ClientManager.Application
{
    public class CustomerApplication : ICustomerApplication
    {
        private readonly ICustomerService _customerService;
        private readonly IDocumentService _documentService;
        private readonly IValidator<CustomerBaseDto> _customerValidator;
        private readonly IWelcomeEmailHandler _welcomeEmailHandler;

        public CustomerApplication(
            ICustomerService customerService,
            IDocumentService documentService,
            IValidator<CustomerBaseDto> customerValidator,
            IWelcomeEmailHandler welcomeEmailHandler)
        {
            _customerService = customerService;
            _documentService = documentService;
            _customerValidator = customerValidator;
            _welcomeEmailHandler = welcomeEmailHandler;
        }

        public async Task<ServiceResponse<Guid>> AddCustomerAsync(CreateCustomerDto customerDto)
        {
            var validationResult = await _customerValidator.ValidateAsync(customerDto).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return ServiceResponse<Guid>.Fail(firstError);
            }

            var customer = customerDto.ToModel();
            await _customerService.AddCustomerAsync(customer).ConfigureAwait(false);

            // Send welcome email synchronously (PDF generation + email send)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _welcomeEmailHandler.HandleCustomerCreatedAsync(
                        customer.Id, customer.Name, customer.Email).ConfigureAwait(false);
                }
                catch
                {
                    // Fire-and-forget: email failure should not break the response
                }
            });

            return ServiceResponse<Guid>.Ok(customer.Id, "CustomerInserted");
        }

        public async Task<ServiceResponse<string>> DeleteCustomerByIdAsync(Guid id)
        {
            var documentCount = await _documentService.GetDocumentCountByCustomerIdAsync(id).ConfigureAwait(false);

            if (documentCount > 0)
                return ServiceResponse<string>.Fail("DeleteCustomerWithDocsError");

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

            var customerDto = customer.ToDto(documents);
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

            var customersDto = customers.Select(c => c.ToDto());
            return ServiceResponse<IEnumerable<CustomerDto>>.Ok(customersDto);
        }

        public async Task<ServiceResponse<string>> UpdateCustomerAsync(UpdateCustomerDto customerDto)
        {
            var validationResult = await _customerValidator.ValidateAsync(customerDto).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return ServiceResponse<string>.Fail(firstError);
            }

            var existingCustomer = await _customerService.GetCustomerByIdAsync(customerDto.Id).ConfigureAwait(false);

            if (existingCustomer == null)
                return ServiceResponse<string>.Fail("CustomerNotFound");

            // Update details (Name, Email, Document, Address) via domain method
            var address = customerDto.Address != null ? customerDto.Address.ToModel() : null;
            existingCustomer.UpdateDetails(customerDto.Name, customerDto.Email, customerDto.Document, address);

            // Re-evaluate status as types or documents might have changed (though documents didn't change here, Type might have)
            var documents = await _documentService.GetDocumentsByCustomerIdAsync(customerDto.Id).ConfigureAwait(false);
            existingCustomer.EvaluateVerificationStatus(documents);

            await _customerService.UpdateCustomerAsync(existingCustomer).ConfigureAwait(false);
            return ServiceResponse<string>.Ok(existingCustomer.Id.ToString(), "CustomerUpdated");
        }

        public async Task<ServiceResponse<string>> VerifyCustomerAsync(Guid id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id).ConfigureAwait(false);

            if (customer == null)
                return ServiceResponse<string>.Fail("CustomerNotFound");

            var documents = await _documentService.GetDocumentsByCustomerIdAsync(id).ConfigureAwait(false);
            customer.EvaluateVerificationStatus(documents);

            await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);

            return ServiceResponse<string>.Ok(customer.Status.ToString(), "CustomerStatusEvaluated");
        }
    }
}
