using ClientManager.Api.Controllers;
using ClientManager.Api.Results;
using ClientManager.Application.Dtos.User;
using ClientManager.Application.Interfaces;
using ClientManager.Domain.Core.Responses;
using ClientManager.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ClientManager.Api.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserApplication> _userApplicationMock;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _userApplicationMock = new Mock<IUserApplication>();
        _userController = new UserController(_userApplicationMock.Object);
    }

    [Fact]
    public async Task CreateUser_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            Role = dto.Role.ToString(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var serviceResponse = new ServiceResponse<UserDto>(userDto);

        _userApplicationMock.Setup(x => x.CreateUserAsync(dto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.CreateUser(dto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<UserDto>>().Subject;
        apiResult.Data!.Username.Should().Be(dto.Username);
        apiResult.Data.Email.Should().Be(dto.Email);

        _userApplicationMock.Verify(a => a.CreateUserAsync(dto), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WhenFails_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateUserDto { Username = "", Email = "", Password = "", Role = UserRole.Viewer };
        var serviceResponse = ServiceResponse<UserDto>.Fail("UsernameRequired");

        _userApplicationMock.Setup(x => x.CreateUserAsync(dto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.CreateUser(dto);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetUserById_WhenExists_ShouldReturnUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = id,
            Username = "testuser",
            Email = "test@test.com",
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var serviceResponse = new ServiceResponse<UserDto>(userDto);

        _userApplicationMock.Setup(x => x.GetUserByIdAsync(id))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.GetUserById(id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<UserDto>>().Subject;
        apiResult.Data!.Id.Should().Be(id);
        apiResult.Data.Username.Should().Be("testuser");

        _userApplicationMock.Verify(a => a.GetUserByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WhenNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var serviceResponse = ServiceResponse<UserDto>.Fail("UserNotFound");

        _userApplicationMock.Setup(x => x.GetUserByIdAsync(id))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.GetUserById(id);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnList()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto
            {
                Id = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@test.com",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new UserDto
            {
                Id = Guid.NewGuid(),
                Username = "user2",
                Email = "user2@test.com",
                Role = "Viewer",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };
        var serviceResponse = new ServiceResponse<IEnumerable<UserDto>>(users);

        _userApplicationMock.Setup(x => x.GetUsersAsync())
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.GetUsers();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<IEnumerable<UserDto>>>().Subject;
        apiResult.Data!.Should().HaveCount(2);

        _userApplicationMock.Verify(a => a.GetUsersAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateUserDto
        {
            Username = "updateduser",
            Email = "updated@test.com",
            Password = "NewPassword123",
            Role = UserRole.Manager
        };
        var userDto = new UserDto
        {
            Id = id,
            Username = dto.Username,
            Email = dto.Email,
            Role = dto.Role.ToString(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var serviceResponse = new ServiceResponse<UserDto>(userDto);

        _userApplicationMock.Setup(x => x.UpdateUserAsync(id, dto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.UpdateUser(id, dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<UserDto>>().Subject;
        apiResult.Data!.Username.Should().Be(dto.Username);

        _userApplicationMock.Verify(a => a.UpdateUserAsync(id, dto), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WhenFails_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateUserDto { Username = "", Email = "", Password = "", Role = UserRole.Viewer };
        var serviceResponse = ServiceResponse<UserDto>.Fail("UserNotFound");

        _userApplicationMock.Setup(x => x.UpdateUserAsync(id, dto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.UpdateUser(id, dto);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteUser_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        var id = Guid.NewGuid();
        var serviceResponse = ServiceResponse<string>.Ok(id.ToString(), "UserDeactivated");

        _userApplicationMock.Setup(x => x.DeleteUserAsync(id))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.DeleteUser(id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
        apiResult.Data.Should().Be(id.ToString());

        _userApplicationMock.Verify(a => a.DeleteUserAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WhenNotFound_ShouldReturnBadRequest()
    {
        // Arrange
        var id = Guid.NewGuid();
        var serviceResponse = ServiceResponse<string>.Fail("UserNotFound");

        _userApplicationMock.Setup(x => x.DeleteUserAsync(id))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _userController.DeleteUser(id);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
