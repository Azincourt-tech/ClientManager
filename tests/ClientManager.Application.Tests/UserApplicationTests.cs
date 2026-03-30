using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ClientManager.Application.Tests;

public class UserApplicationTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IValidator<CreateUserDto>> _validatorMock;
    private readonly UserApplication _userApplication;

    public UserApplicationTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _validatorMock = new Mock<IValidator<CreateUserDto>>();

        _userApplication = new UserApplication(
            _userServiceMock.Object,
            _validatorMock.Object
        );
    }

    [Fact]
    public async Task CreateUserAsync_WhenValid_ShouldCreateUser()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };

        _validatorMock.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null);
        _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userApplication.CreateUserAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Username.Should().Be(dto.Username);
        result.Data.Email.Should().Be(dto.Email);
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WhenValidationFails_ShouldReturnFail()
    {
        // Arrange
        var dto = new CreateUserDto { Username = "", Email = "", Password = "", Role = UserRole.Viewer };

        _validatorMock.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Username", "UsernameRequired") }));

        // Act
        var result = await _userApplication.CreateUserAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UsernameRequired");
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WhenUsernameExists_ShouldReturnFail()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "existinguser",
            Email = "new@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };

        _validatorMock.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync(new User("existinguser", "existing@test.com", "hash", UserRole.Viewer));

        // Act
        var result = await _userApplication.CreateUserAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UsernameAlreadyExists");
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WhenEmailExists_ShouldReturnFail()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "existing@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };

        _validatorMock.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null);
        _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync(new User("otheruser", "existing@test.com", "hash", UserRole.Viewer));

        // Act
        var result = await _userApplication.CreateUserAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("EmailAlreadyExists");
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenExists_ShouldReturnUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User("testuser", "test@test.com", "hash", UserRole.Admin, id);
        _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(user);

        // Act
        var result = await _userApplication.GetUserByIdAsync(id);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(id);
        result.Data.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenNotFound_ShouldReturnFail()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        // Act
        var result = await _userApplication.GetUserByIdAsync(id);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UserNotFound");
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User("user1", "user1@test.com", "hash1", UserRole.Admin),
            new User("user2", "user2@test.com", "hash2", UserRole.Viewer)
        };
        _userServiceMock.Setup(s => s.GetUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userApplication.GetUsersAsync();

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenExists_ShouldUpdateAndReturnUser()
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
        var existingUser = new User("olduser", "old@test.com", "oldhash", UserRole.Viewer, id);

        _validatorMock.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(existingUser);

        // Act
        var result = await _userApplication.UpdateUserAsync(id, dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Username.Should().Be(dto.Username);
        existingUser.Username.Should().Be(dto.Username);
        _userServiceMock.Verify(s => s.UpdateUserAsync(existingUser), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenNotFound_ShouldReturnFail()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CreateUserDto { Username = "user", Email = "email@test.com", Password = "pass", Role = UserRole.Viewer };

        _validatorMock.Setup(v => v.ValidateAsync(dto, default))
            .ReturnsAsync(new ValidationResult());
        _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        // Act
        var result = await _userApplication.UpdateUserAsync(id, dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UserNotFound");
    }

    [Fact]
    public async Task DeleteUserAsync_WhenExists_ShouldDeactivateUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User("user", "user@test.com", "hash", UserRole.Viewer, id);
        _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(user);

        // Act
        var result = await _userApplication.DeleteUserAsync(id);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(id.ToString());
        user.IsActive.Should().BeFalse();
        _userServiceMock.Verify(s => s.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenNotFound_ShouldReturnFail()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        // Act
        var result = await _userApplication.DeleteUserAsync(id);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UserNotFound");
    }
}
