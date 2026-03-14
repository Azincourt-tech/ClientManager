namespace ShopRavenDb.Domain.Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(Customer customer);

        Task UpdateCustomerAsync(Customer customer);

        Task DeleteCustomerByIdAsync(Guid id);

        Task<IEnumerable<Customer>> GetCustomersAsync();

        Task<Customer?> GetCustomerByIdAsync(Guid id);
    }
}