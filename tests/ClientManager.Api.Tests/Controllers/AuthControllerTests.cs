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

public class AuthControllerTests
{
    private readonly Mock<IAuthApplication> _authApplicationMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authApplicationMock = new Mock<IAuthApplication>();
        _authController = new AuthController(_authApplicationMock.Object);
    }

    [Fact]
    public async Task Register_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Password123",
            Role = UserRole.Viewer
        };
        var authResponse = new AuthResponseDto
        {
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            Expiration = DateTimeOffset.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                Role = dto.Role.ToString(),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };
        var serviceResponse = new ServiceResponse<AuthResponseDto>(authResponse);

        _authApplicationMock.Setup(x => x.RegisterAsync(dto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _authController.Register(dto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<AuthResponseDto>>().Subject;
        apiResult.Data!.Token.Should().Be("jwt-token");
        apiResult.Data.User.Username.Should().Be(dto.Username);

        _authApplicationMock.Verify(a => a.RegisterAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Register_WhenFails_ShouldReturnBadRequest()
    {
        // Arrange
        var dto = new CreateUserDto
        {
            Username = "existing",
            Email = "existing@test.com",
            Password = "pass",
            Role = UserRole.Viewer
        };
        var serviceResponse = ServiceResponse<AuthResponseDto>.Fail("UsernameAlreadyExists");

        _authApplicationMock.Setup(x => x.RegisterAsync(dto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _authController.Register(dto);

        // Assert
        result.Should().NotBeNull();
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        _authApplicationMock.Verify(a => a.RegisterAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Login_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "Password123" };
        var authResponse = new AuthResponseDto
        {
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            Expiration = DateTimeOffset.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = loginDto.Username,
                Email = "test@test.com",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };
        var serviceResponse = new ServiceResponse<AuthResponseDto>(authResponse);

        _authApplicationMock.Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<AuthResponseDto>>().Subject;
        apiResult.Data!.Token.Should().Be("jwt-token");

        _authApplicationMock.Verify(a => a.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Login_WhenFails_ShouldReturnBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "wrong", Password = "wrong" };
        var serviceResponse = ServiceResponse<AuthResponseDto>.Fail("InvalidCredentials");

        _authApplicationMock.Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task Refresh_WhenSuccessful_ShouldReturnOk()
    {
        // Arrange
        var refreshToken = "some-refresh-token";
        var authResponse = new AuthResponseDto
        {
            Token = "new-jwt-token",
            RefreshToken = "new-refresh-token",
            Expiration = DateTimeOffset.UtcNow.AddHours(1)
        };
        var serviceResponse = new ServiceResponse<AuthResponseDto>(authResponse);

        _authApplicationMock.Setup(x => x.RefreshTokenAsync(refreshToken))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _authController.Refresh(refreshToken);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<AuthResponseDto>>().Subject;
        apiResult.Data!.Token.Should().Be("new-jwt-token");
    }

    [Fact]
    public async Task Refresh_WhenFails_ShouldReturnBadRequest()
    {
        // Arrange
        var serviceResponse = ServiceResponse<AuthResponseDto>.Fail("RefreshTokenRequired");

        _authApplicationMock.Setup(x => x.RefreshTokenAsync(""))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _authController.Refresh("");

        // Assert
        var badResult = result as BadRequestObjectResult;
        badResult.Should().NotBeNull();
        badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }
}
