namespace ShopRavenDb.Domain.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task AddCustomerAsync(Customer customer);

        Task UpdateCustomerAsync(Customer customer);

        Task DeleteCustomerByIdAsync(Guid id);

        Task<IEnumerable<Customer>> GetCustomersAsync();

        Task<Customer?> GetCustomerByIdAsync(Guid id);
    }
}
