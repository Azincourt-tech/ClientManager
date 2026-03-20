using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using Xunit;

namespace ClientManager.Domain.Tests.Model
{
    public class CustomerTests
    {
        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var name = "John Doe";
            var email = "john@example.com";
            var birthDate = new DateTimeOffset(1990, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var document = "123.456.789-00";
            var type = CustomerType.NaturalPerson;

            // Act
            var customer = new Customer(name, email, birthDate, document, type);

            // Assert
            customer.Name.Should().Be(name);
            customer.Email.Should().Be(email);
            customer.BirthDate.Should().Be(birthDate);
            customer.Document.Should().Be("12345678900");
            customer.Type.Should().Be(type);
            customer.Status.Should().Be(CustomerStatus.Pending);
            customer.IsDeleted.Should().BeFalse();
        }

        [Theory]
        [InlineData("123.456.789-00", "12345678900")]
        [InlineData("12.345.678/0001-99", "12345678000199")]
        [InlineData("abc-123", "123")]
        public void CleanDocument_ShouldRemoveNonDigits(string input, string expected)
        {
            // Act
            var customer = new Customer("Name", "email@test.com", DateTimeOffset.Now, input, CustomerType.NaturalPerson);

            // Assert
            customer.Document.Should().Be(expected);
        }

        [Fact]
        public void StatusMethods_ShouldChangeStatusCorrectly()
        {
            // Arrange
            var customer = new Customer("Name", "email@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);

            // Act & Assert
            customer.Activate();
            customer.Status.Should().Be(CustomerStatus.Active);

            customer.Deactivate();
            customer.Status.Should().Be(CustomerStatus.Inactive);

            customer.SetAttention();
            customer.Status.Should().Be(CustomerStatus.Attention);

            customer.SetVerified();
            customer.Status.Should().Be(CustomerStatus.Verified);

            customer.SetPending();
            customer.Status.Should().Be(CustomerStatus.Pending);
        }

        [Fact]
        public void Delete_ShouldSetIsDeletedToTrue()
        {
            // Arrange
            var customer = new Customer("Name", "email@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);

            // Act
            customer.Delete();

            // Assert
            customer.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void EvaluateVerificationStatus_NaturalPerson_WhenRequirementsMet_ShouldBeVerified()
        {
            // Arrange
            var customer = new Customer("PF", "pf@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);
            var doc1 = new Document("ID", customer.Id, DocumentType.Identity, DateTimeOffset.Now.AddYears(1));
            doc1.Verify();
            var doc2 = new Document("Address", customer.Id, DocumentType.AddressProof, DateTimeOffset.Now.AddYears(1));
            doc2.Verify();

            var documents = new List<Document> { doc1, doc2 };

            // Act
            customer.EvaluateVerificationStatus(documents);

            // Assert
            customer.Status.Should().Be(CustomerStatus.Verified);
        }

        [Fact]
        public void EvaluateVerificationStatus_NaturalPerson_WhenMissingDocument_ShouldBeAttention()
        {
            // Arrange
            var customer = new Customer("PF", "pf@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);
            var documents = new List<Document>
            {
                new Document("ID", customer.Id, DocumentType.Identity, DateTimeOffset.Now.AddYears(1))
                // Missing AddressProof
            };

            // Act
            customer.EvaluateVerificationStatus(documents);

            // Assert
            customer.Status.Should().Be(CustomerStatus.Attention);
        }

        [Fact]
        public void EvaluateVerificationStatus_NaturalPerson_WhenDocumentExpired_ShouldBeAttention()
        {
            // Arrange
            var customer = new Customer("PF", "pf@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);
            var documents = new List<Document>
            {
                new Document("ID", customer.Id, DocumentType.Identity, DateTimeOffset.Now.AddDays(-1)), // Expired
                new Document("Address", customer.Id, DocumentType.AddressProof, DateTimeOffset.Now.AddYears(1))
            };

            // Act
            customer.EvaluateVerificationStatus(documents);

            // Assert
            customer.Status.Should().Be(CustomerStatus.Attention);
        }

        [Fact]
        public void EvaluateVerificationStatus_LegalEntity_WhenRequirementsMet_ShouldBeVerified()
        {
            // Arrange
            var customer = new Customer("PJ", "pj@test.com", DateTimeOffset.Now, "123", CustomerType.LegalEntity);
            var doc1 = new Document("ID", customer.Id, DocumentType.Identity, DateTimeOffset.Now.AddYears(1));
            doc1.Verify();
            var doc2 = new Document("Address", customer.Id, DocumentType.AddressProof, DateTimeOffset.Now.AddYears(1));
            doc2.Verify();
            var doc3 = new Document("Contract", customer.Id, DocumentType.SocialContract, DateTimeOffset.Now.AddYears(1));
            doc3.Verify();

            var documents = new List<Document> { doc1, doc2, doc3 };

            // Act
            customer.EvaluateVerificationStatus(documents);

            // Assert
            customer.Status.Should().Be(CustomerStatus.Verified);
        }

        [Fact]
        public void EvaluateVerificationStatus_LegalEntity_WhenMissingDocument_ShouldBeAttention()
        {
            // Arrange
            var customer = new Customer("PJ", "pj@test.com", DateTimeOffset.Now, "123", CustomerType.LegalEntity);
            var documents = new List<Document>
            {
                new Document("ID", customer.Id, DocumentType.Identity, DateTimeOffset.Now.AddYears(1)),
                new Document("Address", customer.Id, DocumentType.AddressProof, DateTimeOffset.Now.AddYears(1))
                // Missing SocialContract
            };

            // Act
            customer.EvaluateVerificationStatus(documents);

            // Assert
            customer.Status.Should().Be(CustomerStatus.Attention);
        }

        [Fact]
        public void EvaluateVerificationStatus_WhenInactive_ShouldNotChangeStatus()
        {
            // Arrange
            var customer = new Customer("Inactive", "test@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);
            customer.Deactivate(); // Status = Inactive
            var documents = new List<Document>
            {
                new Document("ID", customer.Id, DocumentType.Identity, DateTimeOffset.Now.AddYears(1)),
                new Document("Address", customer.Id, DocumentType.AddressProof, DateTimeOffset.Now.AddYears(1))
            };

            // Act
            customer.EvaluateVerificationStatus(documents);

            // Assert
            customer.Status.Should().Be(CustomerStatus.Inactive);
        }

        [Fact]
        public void EvaluateVerificationStatus_WhenNoDocuments_ShouldBePending()
        {
            // Arrange
            var customer = new Customer("PF", "test@test.com", DateTimeOffset.Now, "123", CustomerType.NaturalPerson);
            customer.SetVerified(); // Manually set to verified to see if it changes back to pending

            // Act
            customer.EvaluateVerificationStatus(new List<Document>());

            // Assert
            customer.Status.Should().Be(CustomerStatus.Pending);
        }
    }
}
