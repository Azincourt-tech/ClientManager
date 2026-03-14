using ClientManager.Domain.Core.Responses;

namespace ClientManager.Api.Controllers
{
    /// <summary>
    /// Manage customers in the Shop system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
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
        public IActionResult GetWelcome()
        {
            return Ok(_localizer["Welcome"].Value);
        }

        /// <summary>
        /// Registers a new customer in the system.
        /// </summary>
        /// <param name="customerDto">The customer data to be inserted.</param>
        /// <returns>A service response containing the new customer ID.</returns>
        [HttpPost("customer", Name = "add-customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<Guid>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCustomer(CustomerDto customerDto)
        {
            var response = await _customerApplication.AddCustomerAsync(customerDto).ConfigureAwait(false);
            if (!response.Success)
            {
                response.Message = _localizer[response.Message];
                return BadRequest(response);
            }
            response.Message = _localizer[response.Message];
            return Ok(response);
        }

        /// <summary>
        /// Updates an existing customer's information.
        /// </summary>
        /// <param name="customerDto">The updated customer data.</param>
        /// <returns>A service response indicating success or failure of the update.</returns>
        [HttpPut("customer", Name = "update-customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomer(CustomerDto customerDto)
        {
            var response = await _customerApplication.UpdateCustomerAsync(customerDto).ConfigureAwait(false);
            if (!response.Success)
            {
                response.Message = _localizer[response.Message];
                return BadRequest(response);
            }
            response.Message = _localizer[response.Message];
            return Ok(response);
        }

        /// <summary>
        /// Removes a customer from the database by their ID.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer (URL encoded).</param>
        /// <returns>A service response confirming the deletion.</returns>
        [HttpDelete("customer/{customerId}", Name = "delete-customer-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomerById(Guid customerId)
        {
            var response = await _customerApplication.DeleteCustomerByIdAsync(customerId).ConfigureAwait(false);
            if (!response.Success)
            {
                response.Message = _localizer[response.Message];
                return NotFound(response);
            }
            response.Message = _localizer[response.Message];
            return Ok(response);
        }

        /// <summary>
        /// Retrieves a list of all registered customers.
        /// </summary>
        /// <returns>A list of customers wrapped in a service response.</returns>
        [HttpGet("customers", Name = "get-customers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<IEnumerable<CustomerDto>>))]
        public async Task<IActionResult> GetCustomers()
        {
            var response = await _customerApplication.GetCustomersAsync().ConfigureAwait(false);
            return Ok(response);
        }

        /// <summary>
        /// Gets the details of a specific customer by their ID.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>The customer data if found.</returns>
        [HttpGet("customers/{customerId}", Name = "get-customer-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<CustomerDto?>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerById(Guid customerId)
        {
            var response = await _customerApplication.GetCustomerByIdAsync(customerId).ConfigureAwait(false);

            if (!response.Success)
            {
                response.Message = _localizer[response.Message];
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Manually triggers verification status re-evaluation for a customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>The updated status of the customer.</returns>
        [HttpPost("customers/{customerId}/verify", Name = "verify-customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyCustomer(Guid customerId)
        {
            var response = await _customerApplication.VerifyCustomerAsync(customerId).ConfigureAwait(false);

            if (!response.Success)
            {
                response.Message = _localizer[response.Message];
                return NotFound(response);
            }

            // Formata a mensagem traduzida com o valor do status (que vem em response.Data)
            response.Message = string.Format(_localizer[response.Message].Value, response.Data);

            return Ok(response);
        }    }
}

