namespace ShopRavenDb.Application.Interfaces
{
    public interface ICustomerApplication
    {
        Task AddCustomerAsync(CustomerDto customerDto);

        Task UpdateCustomerAsync(CustomerDto customerDto);

        Task DeleteCustomerByIdAsync(string id);

        Task<IEnumerable<CustomerDto>> GetCustomersAsync();

        Task<CustomerDto?> GetCustomerByIdAsync(string id);
    }
}