namespace ShopRavenDb.Domain.Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(Customer customer);

        Task UpdateCustomerAsync(Customer customer);

        Task DeleteCustomerByIdAsync(string id);

        Task<IEnumerable<Customer>> GetCustomersAsync();

        Task<Customer?> GetCustomerByIdAsync(string id);
    }
}