using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using Xunit;

namespace ClientManager.Domain.Tests.Model
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var username = "testuser";
            var email = "test@example.com";
            var passwordHash = "hashedpassword123";
            var role = UserRole.Admin;

            // Act
            var user = new User(username, email, passwordHash, role);

            // Assert
            user.Username.Should().Be(username);
            user.Email.Should().Be(email);
            user.PasswordHash.Should().Be(passwordHash);
            user.Role.Should().Be(role);
            user.IsActive.Should().BeTrue();
            user.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Constructor_WithId_ShouldUseProvidedId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var user = new User("test", "test@test.com", "hash", UserRole.Viewer, id);

            // Assert
            user.Id.Should().Be(id);
        }

        [Fact]
        public void Constructor_WithoutId_ShouldGenerateNewId()
        {
            // Act
            var user = new User("test", "test@test.com", "hash", UserRole.Viewer);

            // Assert
            user.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var user = new User("test", "test@test.com", "hash", UserRole.Viewer);
            user.Deactivate();

            // Act
            user.Activate();

            // Assert
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var user = new User("test", "test@test.com", "hash", UserRole.Viewer);

            // Act
            user.Deactivate();

            // Assert
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void UpdateDetails_ShouldUpdateUsernameEmailAndRole()
        {
            // Arrange
            var user = new User("olduser", "old@test.com", "hash", UserRole.Viewer);
            var newUsername = "newuser";
            var newEmail = "new@test.com";
            var newRole = UserRole.Admin;

            // Act
            user.UpdateDetails(newUsername, newEmail, newRole);

            // Assert
            user.Username.Should().Be(newUsername);
            user.Email.Should().Be(newEmail);
            user.Role.Should().Be(newRole);
        }

        [Fact]
        public void UpdatePasswordHash_ShouldUpdatePasswordHash()
        {
            // Arrange
            var user = new User("test", "test@test.com", "oldhash", UserRole.Viewer);
            var newHash = "newhashedpassword456";

            // Act
            user.UpdatePasswordHash(newHash);

            // Assert
            user.PasswordHash.Should().Be(newHash);
        }

        [Theory]
        [InlineData(UserRole.Admin)]
        [InlineData(UserRole.Manager)]
        [InlineData(UserRole.Viewer)]
        public void Constructor_ShouldAcceptAllRoles(UserRole role)
        {
            // Act
            var user = new User("test", "test@test.com", "hash", role);

            // Assert
            user.Role.Should().Be(role);
        }
    }
}
