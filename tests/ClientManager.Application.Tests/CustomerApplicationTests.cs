using ClientManager.Application;
using ClientManager.Application.Dtos.Customer;
using ClientManager.Application.Interfaces;
using ClientManager.Domain.Model;
using ClientManager.Domain.Enums;
using ClientManager.Domain.Core.Interfaces.Services;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace ClientManager.Application.Tests
{
    public class CustomerApplicationTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<IDocumentService> _documentServiceMock;
        private readonly Mock<IValidator<CustomerBaseDto>> _validatorMock;
        private readonly CustomerApplication _customerApplication;

        public CustomerApplicationTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _documentServiceMock = new Mock<IDocumentService>();
            _validatorMock = new Mock<IValidator<CustomerBaseDto>>();

            _customerApplication = new CustomerApplication(
                _customerServiceMock.Object,
                _documentServiceMock.Object,
                _validatorMock.Object
            );
        }

        private AddressDto CreateValidAddressDto() => new AddressDto
        {
            Street = "Main Street",
            Number = 123,
            Complement = "Suite 100",
            City = "New York",
            State = "NY",
            PostalCode = "10001"
        };

        [Fact]
        public async Task AddCustomerAsync_WhenValid_ShouldCallServiceAndReturnOk()
        {
            // Arrange
            var dto = new CreateCustomerDto 
            { 
                Name = "Test Customer", 
                Email = "test@example.com",
                BirthDate = DateTimeOffset.Now.AddYears(-30),
                Document = "12345678901",
                Type = CustomerType.NaturalPerson,
                Address = CreateValidAddressDto()
            };
            
            _validatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _customerApplication.AddCustomerAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            
            _customerServiceMock.Verify(s => s.AddCustomerAsync(It.Is<Customer>(c => 
                c.Name == dto.Name && 
                c.Email == dto.Email && 
                c.Document == dto.Document)), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerByIdAsync_WhenHasDocuments_ShouldReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            _documentServiceMock.Setup(d => d.GetDocumentCountByCustomerIdAsync(id))
                .ReturnsAsync(1);

            // Act
            var result = await _customerApplication.DeleteCustomerByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("DeleteCustomerWithDocsError");

            _documentServiceMock.Verify(d => d.GetDocumentCountByCustomerIdAsync(id), Times.Once);
            _customerServiceMock.Verify(s => s.DeleteCustomerByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_WhenCustomerExists_ShouldReturnCustomerAndReevaluateStatus()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", CustomerType.NaturalPerson, null, id);
            
            _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(id))
                .ReturnsAsync(customer);
            _documentServiceMock.Setup(s => s.GetDocumentsByCustomerIdAsync(id))
                .ReturnsAsync(new List<Document>());

            // Act
            var result = await _customerApplication.GetCustomerByIdAsync(id);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            _customerServiceMock.Verify(s => s.UpdateCustomerAsync(customer), Times.Once);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_WhenCustomerNotFound_ShouldReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(id))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _customerApplication.GetCustomerByIdAsync(id);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("CustomerNotFound");
        }

        [Fact]
        public async Task UpdateCustomerAsync_WhenValid_ShouldUpdateAndReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateCustomerDto 
            { 
                Id = id,
                Name = "Updated Name", 
                Email = "updated@test.com",
                Document = "12345678901",
                Address = CreateValidAddressDto()
            };
            var existingCustomer = new Customer("Old Name", "old@test.com", DateTimeOffset.Now.AddYears(-20), "123", CustomerType.NaturalPerson, null, id);

            _validatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(id))
                .ReturnsAsync(existingCustomer);
            _documentServiceMock.Setup(s => s.GetDocumentsByCustomerIdAsync(id))
                .ReturnsAsync(new List<Document>());

            // Act
            var result = await _customerApplication.UpdateCustomerAsync(dto);

            // Assert
            result.Success.Should().BeTrue();
            existingCustomer.Name.Should().Be(dto.Name);
            _customerServiceMock.Verify(s => s.UpdateCustomerAsync(existingCustomer), Times.Once);
        }

        [Fact]
        public async Task VerifyCustomerAsync_WhenExists_ShouldEvaluateStatus()
        {
            // Arrange
            var id = Guid.NewGuid();
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", CustomerType.NaturalPerson, null, id);

            _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(id))
                .ReturnsAsync(customer);
            _documentServiceMock.Setup(s => s.GetDocumentsByCustomerIdAsync(id))
                .ReturnsAsync(new List<Document>());

            // Act
            var result = await _customerApplication.VerifyCustomerAsync(id);

            // Assert
            result.Success.Should().BeTrue();
            _customerServiceMock.Verify(s => s.UpdateCustomerAsync(customer), Times.Once);
        }
    }
}
