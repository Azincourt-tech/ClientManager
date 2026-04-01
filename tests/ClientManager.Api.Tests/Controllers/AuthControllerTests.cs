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
    public class AuthControllerTests
    {
        private readonly Mock<IAuthApplication> _authApplicationMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authApplicationMock = new Mock<IAuthApplication>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            _authController = new AuthController(
                _authApplicationMock.Object,
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

        private LoginDto CreateValidLoginDto() => new LoginDto
        {
            Username = "testuser",
            Password = "password123"
        };

        [Fact]
        public async Task Register_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var userDto = CreateValidUserDto();
            var authResponse = new AuthResponseDto
            {
                Token = "jwt-token",
                RefreshToken = "refresh-token",
                User = new UserDto { Id = Guid.NewGuid(), Username = userDto.Username, Email = userDto.Email, Role = userDto.Role, IsActive = true },
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(60)
            };
            var serviceResponse = new ServiceResponse<AuthResponseDto>(authResponse, "UserRegistered");

            _authApplicationMock.Setup(x => x.RegisterAsync(userDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _authController.Register(userDto);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<RegisterResponseDto>>().Subject;
            apiResult.Data.Should().NotBeNull();
            apiResult.Data!.User.Should().NotBeNull();
            apiResult.Data.User.Username.Should().Be(userDto.Username);

            _authApplicationMock.Verify(a => a.RegisterAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task Register_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var userDto = CreateValidUserDto();
            var serviceResponse = ServiceResponse<AuthResponseDto>.Fail("UsernameAlreadyExists");

            _authApplicationMock.Setup(x => x.RegisterAsync(userDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _authController.Register(userDto);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            badResult.Value.Should().BeOfType<ApiBadRequestResult>();

            _authApplicationMock.Verify(a => a.RegisterAsync(userDto), Times.Once);
        }

        [Fact]
        public async Task Login_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var loginDto = CreateValidLoginDto();
            var authResponse = new AuthResponseDto
            {
                Token = "jwt-token",
                RefreshToken = "refresh-token",
                User = new UserDto { Id = Guid.NewGuid(), Username = loginDto.Username, IsActive = true },
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(60)
            };
            var serviceResponse = new ServiceResponse<AuthResponseDto>(authResponse, "LoginSuccessful");

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
        public async Task Login_WhenInvalidCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = CreateValidLoginDto();
            var serviceResponse = ServiceResponse<AuthResponseDto>.Fail("InvalidCredentials");

            _authApplicationMock.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _authController.Login(loginDto);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _authApplicationMock.Verify(a => a.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var authResponse = new AuthResponseDto
            {
                Token = "new-jwt-token",
                RefreshToken = "new-refresh-token",
                User = new UserDto { Id = Guid.NewGuid(), IsActive = true },
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(60)
            };
            var serviceResponse = new ServiceResponse<AuthResponseDto>(authResponse, "TokenRefreshed");

            _authApplicationMock.Setup(x => x.RefreshTokenAsync(refreshToken))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _authController.RefreshToken(refreshToken);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<AuthResponseDto>>().Subject;
            apiResult.Data!.Token.Should().Be("new-jwt-token");

            _authApplicationMock.Verify(a => a.RefreshTokenAsync(refreshToken), Times.Once);
        }

        [Fact]
        public async Task RefreshToken_WhenInvalidToken_ShouldReturnBadRequest()
        {
            // Arrange
            var refreshToken = "invalid-refresh-token";
            var serviceResponse = ServiceResponse<AuthResponseDto>.Fail("InvalidRefreshToken");

            _authApplicationMock.Setup(x => x.RefreshTokenAsync(refreshToken))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _authController.RefreshToken(refreshToken);

            // Assert
            result.Should().NotBeNull();
            var badResult = result as BadRequestObjectResult;
            badResult.Should().NotBeNull();
            badResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            _authApplicationMock.Verify(a => a.RefreshTokenAsync(refreshToken), Times.Once);
        }
    }
}
