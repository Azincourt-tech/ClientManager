using ClientManager.Domain.Core.Interfaces.Repositories;
using ClientManager.Domain.Model;
using FluentAssertions;
using Moq;
using Xunit;

namespace ClientManager.Domain.Services.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task AddUserAsync_ShouldCallRepository()
    {
        // Arrange
        var user = new User("testuser", "test@test.com", "hash", Enums.UserRole.Admin);

        // Act
        await _userService.AddUserAsync(user);

        // Assert
        _userRepositoryMock.Verify(r => r.AddUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldCallRepository()
    {
        // Arrange
        var user = new User("testuser", "test@test.com", "hash", Enums.UserRole.Admin);

        // Act
        await _userService.UpdateUserAsync(user);

        // Assert
        _userRepositoryMock.Verify(r => r.UpdateUserAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUserFromRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User("testuser", "test@test.com", "hash", Enums.UserRole.Admin, id);
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        result.Should().Be(user);
        _userRepositoryMock.Verify(r => r.GetUserByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ShouldReturnUserFromRepository()
    {
        // Arrange
        var user = new User("testuser", "test@test.com", "hash", Enums.UserRole.Admin);
        _userRepositoryMock.Setup(r => r.GetUserByUsernameAsync("testuser")).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByUsernameAsync("testuser");

        // Assert
        result.Should().Be(user);
        _userRepositoryMock.Verify(r => r.GetUserByUsernameAsync("testuser"), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUserFromRepository()
    {
        // Arrange
        var user = new User("testuser", "test@test.com", "hash", Enums.UserRole.Admin);
        _userRepositoryMock.Setup(r => r.GetUserByEmailAsync("test@test.com")).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync("test@test.com");

        // Assert
        result.Should().Be(user);
        _userRepositoryMock.Verify(r => r.GetUserByEmailAsync("test@test.com"), Times.Once);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsersFromRepository()
    {
        // Arrange
        var users = new List<User>
        {
            new User("user1", "user1@test.com", "hash1", Enums.UserRole.Admin),
            new User("user2", "user2@test.com", "hash2", Enums.UserRole.Viewer)
        };
        _userRepositoryMock.Setup(r => r.GetUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        result.Should().BeEquivalentTo(users);
        _userRepositoryMock.Verify(r => r.GetUsersAsync(), Times.Once);
    }
}
