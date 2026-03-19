using ClientManager.Domain.Core.Responses;

namespace ClientManager.Application.Interfaces
{
    public interface ICustomerApplication
    {
        Task<ServiceResponse<Guid>> AddCustomerAsync(CreateCustomerDto customerDto);

        Task<ServiceResponse<string>> UpdateCustomerAsync(UpdateCustomerDto customerDto);
        Task<ServiceResponse<string>> VerifyCustomerAsync(Guid id);

        Task<ServiceResponse<string>> DeleteCustomerByIdAsync(Guid id);

        Task<ServiceResponse<IEnumerable<CustomerDto>>> GetCustomersAsync();

        Task<ServiceResponse<CustomerDto?>> GetCustomerByIdAsync(Guid id);
    }
}
