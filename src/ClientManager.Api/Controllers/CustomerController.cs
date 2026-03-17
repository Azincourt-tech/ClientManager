using ClientManager.Api.Results;
using ClientManager.Domain.Core.Responses;

namespace ClientManager.Api.Controllers
{
    /// <summary>
    /// Manage customers in the Shop system.
    /// </summary>
    [Route("api/[controller]")]
    public class CustomerController : MainController
    {
        private readonly ICustomerApplication _customerApplication;
        private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

        public CustomerController(ICustomerApplication customerApplication, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
        {
            _customerApplication = customerApplication;
            _localizer = localizer;
        }

        /// <summary>
        /// Returns a localized welcome message.
        /// </summary>
        [HttpGet("welcome")]
        [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
        public IActionResult GetWelcome()
        {
            return ServiceResponse(new ServiceResponse<string>(_localizer["Welcome"].Value));
        }

        /// <summary>
        /// Registers a new customer in the system.
        /// </summary>
        /// <param name="customerDto">The customer data to be inserted.</param>
        /// <returns>A service response containing the new customer ID.</returns>
        [HttpPost("customer", Name = "add-customer")]
        [ProducesResponseType(typeof(ApiOkResult<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCustomer(CreateCustomerDto customerDto)
        {
            var response = await _customerApplication.AddCustomerAsync(customerDto).ConfigureAwait(false);
            return ServiceResponse(response);
        }

        /// <summary>
        /// Updates an existing customer's information.
        /// </summary>
        /// <param name="id">The unique identifier of the customer.</param>
        /// <param name="customerDto">The updated customer data.</param>
        /// <returns>A service response indicating success or failure of the update.</returns>
        [HttpPut("customer/{id}", Name = "update-customer")]
        [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomer(Guid id, UpdateCustomerDto customerDto)
        {
            customerDto.Id = id;
            var response = await _customerApplication.UpdateCustomerAsync(customerDto).ConfigureAwait(false);
            return ServiceResponse(response);
        }

        /// <summary>
        /// Removes a customer from the database by their ID.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer (URL encoded).</param>
        /// <returns>A service response confirming the deletion.</returns>
        [HttpDelete("customer/{customerId}", Name = "delete-customer-by-id")]
        [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCustomerById(Guid customerId)
        {
            var response = await _customerApplication.DeleteCustomerByIdAsync(customerId).ConfigureAwait(false);
            return ServiceResponse(response);
        }

        /// <summary>
        /// Retrieves a list of all registered customers.
        /// </summary>
        /// <returns>A list of customers wrapped in a service response.</returns>
        [HttpGet("customers", Name = "get-customers")]
        [ProducesResponseType(typeof(ApiOkResult<IEnumerable<CustomerDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomers()
        {
            var response = await _customerApplication.GetCustomersAsync().ConfigureAwait(false);
            return ServiceResponse(response);
        }

        /// <summary>
        /// Gets the details of a specific customer by their ID.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>The customer data if found.</returns>
        [HttpGet("customers/{customerId}", Name = "get-customer-by-id")]
        [ProducesResponseType(typeof(ApiOkResult<CustomerDto?>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var response = await _customerApplication.GetCustomerByIdAsync(customerId).ConfigureAwait(false);
            return ServiceResponse(response);
        }

        /// <summary>
        /// Manually triggers verification status re-evaluation for a customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>The updated status of the customer.</returns>
        [HttpPost("customers/{customerId}/verify", Name = "verify-customer")]
        [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyCustomer(Guid customerId)
        {
            var response = await _customerApplication.VerifyCustomerAsync(customerId).ConfigureAwait(false);
            return ServiceResponse(response);
        }
    }
}
