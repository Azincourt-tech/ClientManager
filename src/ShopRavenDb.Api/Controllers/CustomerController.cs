using ShopRavenDb.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;
using Microsoft.AspNetCore.Mvc;
using ShopRavenDb.Application.Interfaces;
using ShopRavenDb.Application.Dtos;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCustomer(CustomerDto customerDto)
        {
            var response = await _customerApplication.AddCustomerAsync(customerDto).ConfigureAwait(false);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPut("customer", Name = "update-customer")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCustomer(CustomerDto customerDto)
        {
            var response = await _customerApplication.UpdateCustomerAsync(customerDto).ConfigureAwait(false);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("customer/{customerId}", Name = "delete-customer-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomerById(string customerId)
        {
            var formattedCustomerId = Uri.UnescapeDataString(customerId);
            var response = await _customerApplication.DeleteCustomerByIdAsync(formattedCustomerId).ConfigureAwait(false);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("customers", Name = "get-customers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<IEnumerable<CustomerDto>>))]
        public async Task<IActionResult> GetCustomers()
        {
            var response = await _customerApplication.GetCustomersAsync().ConfigureAwait(false);
            return Ok(response);
        }

        [HttpGet("customers/{customerId}", Name = "get-customer-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<CustomerDto?>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerById(string customerId)
        {
            var formattedCustomerId = Uri.UnescapeDataString(customerId);
            var response = await _customerApplication.GetCustomerByIdAsync(formattedCustomerId).ConfigureAwait(false);

            if (!response.Success)
                return NotFound(response);
            
            return Ok(response);
        }
    }
}
