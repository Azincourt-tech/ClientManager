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
    public class UserApplicationTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IValidator<CreateUserDto>> _createUserValidatorMock;
        private readonly UserApplication _userApplication;

        public UserApplicationTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _createUserValidatorMock = new Mock<IValidator<CreateUserDto>>();

            _userApplication = new UserApplication(
                _userServiceMock.Object,
                _createUserValidatorMock.Object
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
        public async Task AddUserAsync_WhenValid_ShouldReturnSuccess()
        {
            // Arrange
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync((User?)null);
            _userServiceMock.Setup(s => s.GetUserByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userApplication.AddUserAsync(dto);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            result.Message.Should().Be("UserInserted");
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task AddUserAsync_WhenValidationFails_ShouldReturnFail()
        {
            // Arrange
            var dto = CreateValidUserDto();
            var validationFailure = new ValidationResult(new[] { new ValidationFailure("Username", "UsernameRequired") });
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationFailure);

            // Act
            var result = await _userApplication.AddUserAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UsernameRequired");
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_WhenUsernameAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByUsernameAsync(dto.Username))
                .ReturnsAsync(new User(dto.Username, dto.Email, "hash", dto.Role));

            // Act
            var result = await _userApplication.AddUserAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UsernameAlreadyExists");
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task AddUserAsync_WhenEmailAlreadyExists_ShouldReturnFail()
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
            var result = await _userApplication.AddUserAsync(dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("EmailAlreadyExists");
            _userServiceMock.Verify(s => s.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
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
        public async Task GetUserByIdAsync_WhenUserNotFound_ShouldReturnFail()
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
            result.Data.Should().HaveCount(2);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserExists_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = CreateValidUserDto();
            var existingUser = new User("olduser", "old@test.com", "oldhash", UserRole.Viewer, id);

            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(existingUser);

            // Act
            var result = await _userApplication.UpdateUserAsync(id, dto);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("UserUpdated");
            existingUser.Username.Should().Be(dto.Username);
            existingUser.Email.Should().Be(dto.Email);
            _userServiceMock.Verify(s => s.UpdateUserAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserNotFound_ShouldReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = CreateValidUserDto();
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

            // Act
            var result = await _userApplication.UpdateUserAsync(id, dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UserNotFound");
            _userServiceMock.Verify(s => s.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenValidationFails_ShouldReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = CreateValidUserDto();
            var validationFailure = new ValidationResult(new[] { new ValidationFailure("Username", "UsernameRequired") });
            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(validationFailure);

            // Act
            var result = await _userApplication.UpdateUserAsync(id, dto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UsernameRequired");
            _userServiceMock.Verify(s => s.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenPasswordProvided_ShouldUpdatePasswordHash()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = CreateValidUserDto();
            dto.Password = "newpassword456";
            var existingUser = new User("olduser", "old@test.com", "oldhash", UserRole.Viewer, id);

            _createUserValidatorMock.Setup(v => v.ValidateAsync(dto, default))
                .ReturnsAsync(new ValidationResult());
            _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(existingUser);

            // Act
            var result = await _userApplication.UpdateUserAsync(id, dto);

            // Assert
            result.Success.Should().BeTrue();
            existingUser.PasswordHash.Should().NotBe("oldhash");
            _userServiceMock.Verify(s => s.UpdateUserAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_WhenUserExists_ShouldReturnSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = new User("testuser", "test@test.com", "hash", UserRole.Admin, id);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync(user);

            // Act
            var result = await _userApplication.DeleteUserByIdAsync(id);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("UserDeleted");
            _userServiceMock.Verify(s => s.DeleteUserByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteUserByIdAsync_WhenUserNotFound_ShouldReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            _userServiceMock.Setup(s => s.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

            // Act
            var result = await _userApplication.DeleteUserByIdAsync(id);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("UserNotFound");
            _userServiceMock.Verify(s => s.DeleteUserByIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
