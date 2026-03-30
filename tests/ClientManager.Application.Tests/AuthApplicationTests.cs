using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ClientManager.Application.Tests;

public class AuthApplicationTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthApplication _authApplication;

    public AuthApplicationTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _configurationMock = new Mock<IConfiguration>();

        var configSection = new Mock<IConfigurationSection>();
        configSection.Setup(s => s.Value).Returns("60");
        _configurationMock.Setup(c => c["JwtSettings:ExpirationInMinutes"]).Returns("60");

        _authApplication = new AuthApplication(
            _userServiceMock.Object,
            _tokenServiceMock.Object,
            _configurationMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_WhenValid_ShouldCreateUserAndReturnToken()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null);
        _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-jwt-token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("test-refresh-token");

        // Act
        var result = await _authApplication.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be("test-jwt-token");
        result.Data.RefreshToken.Should().Be("test-refresh-token");
        result.Data.User.Username.Should().Be(dto.Username);
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ShouldReturnFail()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "existinguser",
            Email = "new@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync(new User("existinguser", "existing@test.com", "hash", UserRole.Viewer));

        // Act
        var result = await _authApplication.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UsernameAlreadyExists");
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ShouldReturnFail()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "existing@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
            .ReturnsAsync((User?)null);
        _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync(new User("otheruser", "existing@test.com", "hash", UserRole.Viewer));

        // Act
        var result = await _authApplication.RegisterAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("EmailAlreadyExists");
        _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsValid_ShouldReturnToken()
    {
        // Arrange
        var password = "Password123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User("testuser", "test@test.com", passwordHash, UserRole.Admin);

        var loginDto = new LoginDto { Username = "testuser", Password = password };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync("testuser"))
            .ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("test-jwt-token");
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("test-refresh-token");

        // Act
        var result = await _authApplication.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Token.Should().Be("test-jwt-token");
        result.Data.User.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnFail()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "nonexistent", Password = "pass" };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authApplication.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("InvalidCredentials");
    }

    [Fact]
    public async Task LoginAsync_WhenUserInactive_ShouldReturnFail()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User("testuser", "test@test.com", passwordHash, UserRole.Admin);
        user.Deactivate();

        var loginDto = new LoginDto { Username = "testuser", Password = "Password123" };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _authApplication.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("UserInactive");
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordWrong_ShouldReturnFail()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User("testuser", "test@test.com", passwordHash, UserRole.Admin);

        var loginDto = new LoginDto { Username = "testuser", Password = "WrongPassword" };

        _userServiceMock.Setup(s => s.GetUserByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _authApplication.LoginAsync(loginDto);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("InvalidCredentials");
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenValid_ShouldReturnNewToken()
    {
        // Arrange
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
            .Returns("new-refresh-token");

        // Act
        var result = await _authApplication.RefreshTokenAsync("some-refresh-token");

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Token.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task RefreshTokenAsync_WhenEmpty_ShouldReturnFail()
    {
        // Act
        var result = await _authApplication.RefreshTokenAsync("");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("RefreshTokenRequired");
    }
}
