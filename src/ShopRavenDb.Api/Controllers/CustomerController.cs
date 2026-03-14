namespace ShopRavenDb.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerApplication _customerApplication;

        public CustomerController(ICustomerApplication customerApplication)
        {
            _customerApplication = customerApplication;
        }

        [HttpPost("customer", Name = "add-customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> AddCustomer(CustomerDto customerDto)
        {
            await _customerApplication.AddCustomerAsync(customerDto).ConfigureAwait(false);
            return Ok("Customer Inserted successfully!");
        }

        [HttpPut("customer", Name = "update-customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> UpdateCustomer(CustomerDto customerDto)
        {
            await _customerApplication.UpdateCustomerAsync(customerDto).ConfigureAwait(false);
            return Ok("Customer updated successfully!");
        }

        [HttpDelete("customer/{customerId}", Name = "delete-customer-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> DeleteCustomerById(string customerId)
        {
            var formattedCustomerId = Uri.UnescapeDataString(customerId);
            await _customerApplication.DeleteCustomerByIdAsync(formattedCustomerId).ConfigureAwait(false);
            return Ok("Customer deleted successfully!");
        }


        [HttpGet("customers", Name = "get-customers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            return Ok(await _customerApplication.GetCustomersAsync().ConfigureAwait(false));
        }

        [HttpGet("customers/{customerId}", Name = "get-customer-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomerById(string customerId)
        {

            var formattedCustomerId = Uri.UnescapeDataString(customerId);
            var customer = await _customerApplication.GetCustomerByIdAsync(formattedCustomerId).ConfigureAwait(false);

            if (customer is null)
                return NotFound();
            else
                return Ok(customer);
        }

    }
}
