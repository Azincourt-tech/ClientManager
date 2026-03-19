using ClientManager.Domain.Model;
using FluentAssertions;
using Xunit;

namespace ClientManager.Domain.Tests.Model
{
    public class AddressTests
    {
        [Fact]
        public void Address_WhenCreated_ShouldHaveValidProperties()
        {
            // Arrange
            var street = "Main St";
            var number = 123;
            var complement = "Apt 1";
            var city = "New York";
            var state = "NY";
            var postalCode = "10001";

            // Act
            var address = new Address(street, number, complement, city, state, postalCode);

            // Assert
            address.Street.Should().Be(street);
            address.Number.Should().Be(number);
            address.Complement.Should().Be(complement);
            address.City.Should().Be(city);
            address.State.Should().Be(state);
            address.PostalCode.Should().Be(postalCode);
            address.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("Main Street", 123, "Apt 1", "New York", "NY", "12345")]
        [InlineData("Second Avenue", 456, "Suite 200", "Los Angeles", "CA", "90001")]
        [InlineData("Third Street", 789, "", "Chicago", "IL", "60601")]
        public void Address_Constructor_WhenParametersAreProvided_ShouldInitializeProperties(
            string street, 
            int number, 
            string complement, 
            string city, 
            string state, 
            string postalCode)
        {
            // Act
            var address = new Address(street, number, complement, city, state, postalCode);

            // Assert
            address.Should().NotBeNull();
            address.Street.Should().Be(street);
            address.Number.Should().Be(number);
            address.Complement.Should().Be(complement);
            address.City.Should().Be(city);
            address.State.Should().Be(state);
            address.PostalCode.Should().Be(postalCode);
            address.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Address_WhenDeactivated_ShouldBeInactive()
        {
            // Arrange
            var address = new Address("Street", 1, "C", "City", "ST", "00000");

            // Act
            address.Deactivate();

            // Assert
            address.IsActive.Should().BeFalse();
        }
    }
}
