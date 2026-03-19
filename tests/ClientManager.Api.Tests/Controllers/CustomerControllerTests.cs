using ClientManager.Api.Controllers;
using ClientManager.Api.Results;
using ClientManager.Application.Dtos.Customer;
using ClientManager.Application.Dtos.Document;
using ClientManager.Application.Interfaces;
using ClientManager.Domain.Core.Responses;
using ClientManager.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace ClientManager.Api.Tests.Controllers
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerApplication> _customerApplicationMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly CustomerController _customerController;

        public CustomerControllerTests()
        {
            _customerApplicationMock = new Mock<ICustomerApplication>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            _customerController = new CustomerController(
                _customerApplicationMock.Object,
                _localizerMock.Object
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
        public void GetWelcome_ShouldReturnLocalizedMessage()
        {
            // Arrange
            var welcomeMessage = "Welcome to the system";
            var localizedString = new LocalizedString("Welcome", welcomeMessage);
            _localizerMock.Setup(l => l["Welcome"]).Returns(localizedString);

            // Act
            var result = _customerController.GetWelcome();

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            response.Data.Should().Be(welcomeMessage);
        }

        [Fact]
        public async Task AddCustomer_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var customerDto = new CreateCustomerDto
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                BirthDate = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Document = "123456789",
                Type = CustomerType.NaturalPerson,
                Address = CreateValidAddressDto()
            };
            var expectedId = Guid.NewGuid();
            var serviceResponse = new ServiceResponse<Guid>(expectedId);

            _customerApplicationMock.Setup(x => x.AddCustomerAsync(customerDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.AddCustomer(customerDto);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<Guid>>().Subject;
            apiResult.Data.Should().Be(expectedId);

            _customerApplicationMock.Verify(a => a.AddCustomerAsync(It.Is<CreateCustomerDto>(d =>
                d.Name == customerDto.Name &&
                d.Email == customerDto.Email &&
                d.Document == customerDto.Document &&
                d.Address.Street == customerDto.Address.Street)), Times.Once);
        }

        [Fact]
        public async Task AddCustomer_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var customerDto = new CreateCustomerDto
            {
                Name = "Invalid",
                Email = "invalid",
                Address = CreateValidAddressDto()
            };
            var serviceResponse = ServiceResponse<Guid>.Fail("Error", "Error adding customer");

            _customerApplicationMock.Setup(x => x.AddCustomerAsync(customerDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.AddCustomer(customerDto);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            badResult.Value.Should().BeOfType<ApiBadRequestResult>();

            _customerApplicationMock.Verify(a => a.AddCustomerAsync(customerDto), Times.Once);
        }

        [Fact]
        public async Task GetCustomerById_WhenExists_ShouldReturnCustomer()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customerDto = new CustomerDto
            {
                Id = customerId,
                Name = "John Doe",
                Email = "john.doe@example.com",
                BirthDate = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Document = "123456789",
                Type = CustomerType.NaturalPerson,
                Status = CustomerStatus.Verified,
                Address = CreateValidAddressDto(),
                Documents = new List<DocumentDto>
                {
                    new DocumentDto { Id = Guid.NewGuid(), Name = "ID.pdf", Type = DocumentType.Identity, CreateDate = DateTimeOffset.Now }
                }
            };
            var serviceResponse = new ServiceResponse<CustomerDto?>(customerDto);

            _customerApplicationMock.Setup(x => x.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.GetCustomerById(customerId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<CustomerDto>>().Subject;
            apiResult.Data!.Id.Should().Be(customerId);
            apiResult.Data.Name.Should().Be("John Doe");
            apiResult.Data.Documents.Should().NotBeEmpty();

            _customerApplicationMock.Verify(a => a.GetCustomerByIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnList()
        {
            // Arrange
            var customers = new List<CustomerDto>
            {
                new CustomerDto
                {
                    Id = Guid.NewGuid(),
                    Name = "User 1",
                    Email = "user1@test.com",
                    Address = CreateValidAddressDto()
                }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<CustomerDto>>(customers);

            _customerApplicationMock.Setup(x => x.GetCustomersAsync())
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.GetCustomers();

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<IEnumerable<CustomerDto>>>().Subject;
            apiResult.Data.Should().NotBeNull();
            apiResult.Data!.Should().HaveCount(1);
            apiResult.Data.First().Name.Should().Be("User 1");

            _customerApplicationMock.Verify(a => a.GetCustomersAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomer_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new UpdateCustomerDto
            {
                Id = id,
                Name = "Updated Name",
                Email = "updated@example.com",
                BirthDate = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Document = "987654321",
                Type = CustomerType.LegalEntity,
                Address = CreateValidAddressDto()
            };
            var serviceResponse = new ServiceResponse<string>("Updated successfully");

            _customerApplicationMock.Setup(x => x.UpdateCustomerAsync(It.Is<UpdateCustomerDto>(d => d.Id == id)))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.UpdateCustomer(id, dto);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be("Updated successfully");

            _customerApplicationMock.Verify(a => a.UpdateCustomerAsync(It.Is<UpdateCustomerDto>(d =>
                d.Id == id &&
                d.Name == dto.Name &&
                d.Email == dto.Email)), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerById_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceResponse = new ServiceResponse<string>("Deleted");

            _customerApplicationMock.Setup(x => x.DeleteCustomerByIdAsync(id))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.DeleteCustomerById(id);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be("Deleted");

            _customerApplicationMock.Verify(a => a.DeleteCustomerByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task VerifyCustomer_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceResponse = new ServiceResponse<string>("Verified");

            _customerApplicationMock.Setup(x => x.VerifyCustomerAsync(id))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _customerController.VerifyCustomer(id);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be("Verified");

            _customerApplicationMock.Verify(a => a.VerifyCustomerAsync(id), Times.Once);
        }
    }
}
