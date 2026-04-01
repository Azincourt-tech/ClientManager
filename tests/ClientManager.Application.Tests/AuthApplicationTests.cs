using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;

namespace ClientManager.Application.Tests
{
    public class AuthApplicationTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IValidator<CreateUserDto>> _createUserValidatorMock;
        private readonly Mock<IValidator<LoginDto>> _loginValidatorMock;
        private readonly AuthApplication _authApplication;

        public AuthApplicationTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _emailServiceMock = new Mock<IEmailService>();
            _createUserValidatorMock = new Mock<IValidator<CreateUserDto>>();
            _loginValidatorMock = new Mock<IValidator<LoginDto>>();

            _authApplication = new AuthApplication(
                _userServiceMock.Object,
                _tokenServiceMock.Object,
                _emailServiceMock.Object,
                _createUserValidatorMock.Object,
                _loginValidatorMock.Object
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
        public async Task RegisterAsync_WhenValid_ShouldReturnSuccessWithToken()
        {
            // Arrange
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);
            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("generated-jwt-token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns("generated-refresh-token");

            // Act
            var result = await _authApplication.RegisterAsync(dto);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("UserRegistered");
            result.Data.Should().NotBeNull();
            result.Data!.Token.Should().Be("generated-jwt-token");
            result.Data.RefreshToken.Should().Be("generated-refresh-token");
            result.Data.User.Username.Should().Be(dto.Username);
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenValidationFails_ShouldReturnFail()
        {
            // Arrange
            var dto = CreateValidUserDto();
            var validationFailure = new ValidationResult(new[] { new ValidationFailure("Username", "UsernameRequired") });
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationFailure);

            // Act
            var result = await _authApplication.RegisterAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UsernameRequired");
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WhenUsernameExists_ShouldReturnFail()
        {
            // Arrange
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync(new User(dto.Username, dto.Email, "hash", dto.Role));

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
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);
            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync(new User("otheruser", dto.Email, "hash", dto.Role));

            // Act
            var result = await _authApplication.RegisterAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("EmailAlreadyExists");
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenValid_ShouldReturnSuccessWithToken()
        {
            // Arrange
            var loginDto = CreateValidLoginDto();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
            var user = new User("testuser", "test@test.com", passwordHash, UserRole.Viewer);

            _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("login-jwt-token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns("login-refresh-token");

            // Act
            var result = await _authApplication.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("LoginSuccessful");
            result.Data!.Token.Should().Be("login-jwt-token");
            result.Data.RefreshToken.Should().Be("login-refresh-token");
        }

        [Fact]
        public async Task LoginAsync_WhenValidationFails_ShouldReturnFail()
        {
            // Arrange
            var loginDto = CreateValidLoginDto();
            var validationFailure = new ValidationResult(new[] { new ValidationFailure("Username", "UsernameRequired") });
            _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
                .ReturnsAsync(validationFailure);

            // Act
            var result = await _authApplication.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UsernameRequired");
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ShouldReturnFail()
        {
            // Arrange
            var loginDto = CreateValidLoginDto();
            _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(loginDto.Username))
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
            var loginDto = CreateValidLoginDto();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
            var user = new User("testuser", "test@test.com", passwordHash, UserRole.Viewer);
            user.Deactivate();

            _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);

            // Act
            var result = await _authApplication.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UserInactive");
        }

        [Fact]
        public async Task LoginAsync_WhenWrongPassword_ShouldReturnFail()
        {
            // Arrange
            var loginDto = CreateValidLoginDto();
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            var user = new User("testuser", "test@test.com", passwordHash, UserRole.Viewer);

            _loginValidatorMock.Setup(v => v.ValidateAsync(loginDto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);

            // Act
            var result = await _authApplication.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("InvalidCredentials");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenValid_ShouldReturnNewTokens()
        {
            // Arrange - register a user first to get a refresh token
            var dto = CreateValidUserDto();
            var user = new User(dto.Username, dto.Email, "hash", dto.Role);
            var userId = user.Id;

            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);
            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token1");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns("refresh-token-1");

            var registerResult = await _authApplication.RegisterAsync(dto);
            var oldRefreshToken = registerResult.Data!.RefreshToken;

            _userServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("new-token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns("new-refresh-token");

            // Act
            var result = await _authApplication.RefreshTokenAsync(oldRefreshToken);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("TokenRefreshed");
            result.Data!.Token.Should().Be("new-token");
            result.Data.RefreshToken.Should().Be("new-refresh-token");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenInvalidToken_ShouldReturnFail()
        {
            // Act
            var result = await _authApplication.RefreshTokenAsync("invalid-refresh-token");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("InvalidRefreshToken");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenUserNotFound_ShouldReturnFail()
        {
            // Arrange - register a user first
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);
            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns("refresh-token-to-use");

            var registerResult = await _authApplication.RegisterAsync(dto);
            var refreshToken = registerResult.Data!.RefreshToken;

            _userServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authApplication.RefreshTokenAsync(refreshToken);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UserNotFound");
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenUserInactive_ShouldReturnFail()
        {
            // Arrange - register a user first
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);
            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken())
                .Returns("inactive-user-refresh");

            var registerResult = await _authApplication.RegisterAsync(dto);
            var refreshToken = registerResult.Data!.RefreshToken;

            var inactiveUser = new User(dto.Username, dto.Email, "hash", dto.Role);
            inactiveUser.Deactivate();
            _userServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(inactiveUser);

            // Act
            var result = await _authApplication.RefreshTokenAsync(refreshToken);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UserNotFound");
        }
    }
}
