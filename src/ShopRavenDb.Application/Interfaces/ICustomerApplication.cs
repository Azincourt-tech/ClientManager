using ShopRavenDb.Application.Dtos;
using ShopRavenDb.Domain.Core.Responses;

namespace ShopRavenDb.Application.Interfaces
{
    public interface ICustomerApplication
    {
        Task<ServiceResponse<string>> AddCustomerAsync(CustomerDto customerDto);

        Task<ServiceResponse<string>> UpdateCustomerAsync(CustomerDto customerDto);

        Task<ServiceResponse<string>> DeleteCustomerByIdAsync(string id);

        Task<ServiceResponse<IEnumerable<CustomerDto>>> GetCustomersAsync();

        Task<ServiceResponse<CustomerDto?>> GetCustomerByIdAsync(string id);
    }
}