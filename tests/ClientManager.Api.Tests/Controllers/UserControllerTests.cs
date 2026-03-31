using ClientManager.Api.Controllers;
using ClientManager.Api.Results;
using ClientManager.Application.Dtos.User;
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
    public class UserControllerTests
    {
        private readonly Mock<IUserApplication> _userApplicationMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _userApplicationMock = new Mock<IUserApplication>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            _userController = new UserController(
                _userApplicationMock.Object,
                _localizerMock.Object
            );
        }

        private CreateUserDto CreateValidUserDto() => new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = UserRole.Viewer
        };

        [Fact]
        public async Task AddUser_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var userDto = CreateValidUserDto();
            var expectedId = Guid.NewGuid();
            var serviceResponse = new ServiceResponse<Guid>(expectedId, "UserInserted");

            _userApplicationMock.Setup(x => x.AddUserAsync(userDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.AddUser(userDto);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<Guid>>().Subject;
            apiResult.Data.Should().Be(expectedId);

            _userApplicationMock.Verify(a => a.AddUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task AddUser_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var userDto = CreateValidUserDto();
            var serviceResponse = ServiceResponse<Guid>.Fail("UsernameAlreadyExists");

            _userApplicationMock.Setup(x => x.AddUserAsync(userDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.AddUser(userDto);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            badResult.Value.Should().BeOfType<ApiBadRequestResult>();

            _userApplicationMock.Verify(a => a.AddUserAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task GetUserById_WhenExists_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDto
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };
            var serviceResponse = new ServiceResponse<UserDto?>(userDto);

            _userApplicationMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.GetUserById(userId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<UserDto>>().Subject;
            apiResult.Data!.Id.Should().Be(userId);
            apiResult.Data.Username.Should().Be("testuser");

            _userApplicationMock.Verify(a => a.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserById_WhenNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var serviceResponse = ServiceResponse<UserDto?>.Fail("UserNotFound");

            _userApplicationMock.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.GetUserById(userId);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _userApplicationMock.Verify(a => a.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnList()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = Guid.NewGuid(), Username = "user1", Email = "user1@test.com", Role = UserRole.Admin, IsActive = true },
                new UserDto { Id = Guid.NewGuid(), Username = "user2", Email = "user2@test.com", Role = UserRole.Viewer, IsActive = true }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<UserDto>>(users);

            _userApplicationMock.Setup(x => x.GetUsersAsync())
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.GetUsers();

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<IEnumerable<UserDto>>>().Subject;
            apiResult.Data.Should().HaveCount(2);
            apiResult.Data!.First().Username.Should().Be("user1");

            _userApplicationMock.Verify(a => a.GetUsersAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userDto = CreateValidUserDto();
            var serviceResponse = new ServiceResponse<string>(id.ToString(), "UserUpdated");

            _userApplicationMock.Setup(x => x.UpdateUserAsync(id, userDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.UpdateUser(id, userDto);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be(id.ToString());

            _userApplicationMock.Verify(a => a.UpdateUserAsync(id, userDto), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenUserNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userDto = CreateValidUserDto();
            var serviceResponse = ServiceResponse<string>.Fail("UserNotFound");

            _userApplicationMock.Setup(x => x.UpdateUserAsync(id, userDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.UpdateUser(id, userDto);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _userApplicationMock.Verify(a => a.UpdateUserAsync(id, userDto), Times.Once);
        }

        [Fact]
        public async Task DeleteUserById_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceResponse = new ServiceResponse<string>(id.ToString(), "UserDeleted");

            _userApplicationMock.Setup(x => x.DeleteUserByIdAsync(id))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.DeleteUserById(id);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be(id.ToString());

            _userApplicationMock.Verify(a => a.DeleteUserByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteUserById_WhenUserNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceResponse = ServiceResponse<string>.Fail("UserNotFound");

            _userApplicationMock.Setup(x => x.DeleteUserByIdAsync(id))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _userController.DeleteUserById(id);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _userApplicationMock.Verify(a => a.DeleteUserByIdAsync(id), Times.Once);
        }
    }
}
