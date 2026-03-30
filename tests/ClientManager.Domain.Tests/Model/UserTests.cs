using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using Xunit;

namespace ClientManager.Domain.Tests.Model;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange
        var username = "admin";
        var email = "admin@test.com";
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
    public void Constructor_WithId_ShouldSetId()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var user = new User("user", "email@test.com", "hash", UserRole.Viewer, id);

        // Assert
        user.Id.Should().Be(id);
    }

    [Fact]
    public void Constructor_WithoutId_ShouldGenerateId()
    {
        // Act
        var user = new User("user", "email@test.com", "hash", UserRole.Viewer);

        // Assert
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        // Arrange
        var user = new User("user", "email@test.com", "hash", UserRole.Viewer);
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
        var user = new User("user", "email@test.com", "hash", UserRole.Viewer);

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateFields()
    {
        // Arrange
        var user = new User("olduser", "old@test.com", "hash", UserRole.Viewer);

        // Act
        user.UpdateDetails("newuser", "new@test.com", UserRole.Admin);

        // Assert
        user.Username.Should().Be("newuser");
        user.Email.Should().Be("new@test.com");
        user.Role.Should().Be(UserRole.Admin);
    }

    [Fact]
    public void UpdatePasswordHash_ShouldUpdateHash()
    {
        // Arrange
        var user = new User("user", "email@test.com", "oldhash", UserRole.Viewer);

        // Act
        user.UpdatePasswordHash("newhash");

        // Assert
        user.PasswordHash.Should().Be("newhash");
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Manager)]
    [InlineData(UserRole.Viewer)]
    public void Constructor_ShouldAcceptAllRoles(UserRole role)
    {
        // Act
        var user = new User("user", "email@test.com", "hash", role);

        // Assert
        user.Role.Should().Be(role);
    }
}
