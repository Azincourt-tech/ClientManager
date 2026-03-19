using ClientManager.Domain.Model;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Core.Interfaces.Repositories;
using Moq;
using Xunit;
using FluentAssertions;

namespace ClientManager.Domain.Services.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepositoryMock.Object);
        }

        [Fact]
        public async Task AddCustomerAsync_ShouldCallRepository()
        {
            // Arrange
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", Enums.CustomerType.NaturalPerson);

            // Act
            await _customerService.AddCustomerAsync(customer);

            // Assert
            _customerRepositoryMock.Verify(r => r.AddCustomerAsync(customer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerByIdAsync_ShouldCallRepository()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            await _customerService.DeleteCustomerByIdAsync(id);

            // Assert
            _customerRepositoryMock.Verify(r => r.DeleteCustomerByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCustomerFromRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", Enums.CustomerType.NaturalPerson, null, id);
            _customerRepositoryMock.Setup(r => r.GetCustomerByIdAsync(id)).ReturnsAsync(customer);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(id);

            // Assert
            result.Should().Be(customer);
            _customerRepositoryMock.Verify(r => r.GetCustomerByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetCustomersAsync_ShouldReturnCustomersFromRepository()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer("Test 1", "test1@test.com", DateTimeOffset.Now.AddYears(-20), "123", Enums.CustomerType.NaturalPerson),
                new Customer("Test 2", "test2@test.com", DateTimeOffset.Now.AddYears(-25), "456", Enums.CustomerType.LegalEntity)
            };
            _customerRepositoryMock.Setup(r => r.GetCustomersAsync()).ReturnsAsync(customers);

            // Act
            var result = await _customerService.GetCustomersAsync();

            // Assert
            result.Should().BeEquivalentTo(customers);
            _customerRepositoryMock.Verify(r => r.GetCustomersAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldCallRepository()
        {
            // Arrange
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", Enums.CustomerType.NaturalPerson);

            // Act
            await _customerService.UpdateCustomerAsync(customer);

            // Assert
            _customerRepositoryMock.Verify(r => r.UpdateCustomerAsync(customer), Times.Once);
        }
    }
}
