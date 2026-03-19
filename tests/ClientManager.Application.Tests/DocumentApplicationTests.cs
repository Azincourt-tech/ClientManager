using ClientManager.Application;
using ClientManager.Application.Dtos.Document;
using ClientManager.Application.Interfaces;
using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using ClientManager.Domain.Core.Responses;
using ClientManager.Domain.Core.Interfaces.Services;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using Raven.Client.Documents.Operations.Attachments;
using Xunit;

namespace ClientManager.Application.Tests
{
    public class DocumentApplicationTests
    {
        private readonly Mock<IDocumentService> _documentServiceMock;
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<IFileValidator> _fileValidatorMock;
        private readonly Mock<IValidator<IFormFile>> _fluentValidatorMock;
        private readonly DocumentApplication _documentApplication;

        public DocumentApplicationTests()
        {
            _documentServiceMock = new Mock<IDocumentService>();
            _customerServiceMock = new Mock<ICustomerService>();
            _fileValidatorMock = new Mock<IFileValidator>();
            _fluentValidatorMock = new Mock<IValidator<IFormFile>>();

            _documentApplication = new DocumentApplication(
                _documentServiceMock.Object,
                _customerServiceMock.Object,
                _fileValidatorMock.Object,
                _fluentValidatorMock.Object
            );
        }

        [Fact]
        public async Task AttachDocumentAsync_WhenValid_ShouldAttachAndReevaluateStatus()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var type = DocumentType.Identity;
            var expiryDate = DateTimeOffset.Now.AddYears(1);
            var documentId = Guid.NewGuid();
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", CustomerType.NaturalPerson, null, customerId);

            _fluentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IFormFile>>(), default))
                .ReturnsAsync(new ValidationResult());
            
            string error;
            _fileValidatorMock.Setup(v => v.IsValid(fileMock.Object, out error))
                .Returns(true);

            _documentServiceMock.Setup(s => s.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate))
                .ReturnsAsync(documentId);

            _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            _documentServiceMock.Setup(s => s.GetDocumentsByCustomerIdAsync(customerId))
                .ReturnsAsync(new List<Document>());

            // Act
            var result = await _documentApplication.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().Be(documentId);

            _documentServiceMock.Verify(s => s.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate), Times.Once);
            _customerServiceMock.Verify(s => s.UpdateCustomerAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task AttachDocumentAsync_WhenFileInvalid_ShouldReturnFail()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var type = DocumentType.Identity;
            
            _fluentValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<IFormFile>>(), default))
                .ReturnsAsync(new ValidationResult());
            
            string errorMessage = "Invalid file type";
            _fileValidatorMock.Setup(v => v.IsValid(fileMock.Object, out errorMessage))
                .Returns(false);

            // Act
            var result = await _documentApplication.AttachDocumentAsync(customerId, fileMock.Object, type);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be(errorMessage);
            _documentServiceMock.Verify(s => s.AttachDocumentAsync(It.IsAny<Guid>(), It.IsAny<IFormFile>(), It.IsAny<DocumentType>(), It.IsAny<DateTimeOffset?>()), Times.Never);
        }

        [Fact]
        public async Task GetAttachDocumentAsync_WhenDocumentNotFound_ShouldReturnFail()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            _documentServiceMock.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync((Document)null);

            // Act
            var result = await _documentApplication.GetAttachDocumentAsync(documentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("DocumentNotFound");
        }

        [Fact]
        public async Task DeleteDocumentAsync_WhenExists_ShouldDeleteAndReevaluateStatus()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var documentId = Guid.NewGuid();
            var customer = new Customer("Test", "test@test.com", DateTimeOffset.Now.AddYears(-20), "123", CustomerType.NaturalPerson, null, customerId);
            var document = new Document("TestDoc", customerId, DocumentType.Identity);

            _documentServiceMock.Setup(s => s.GetDocumentByIdAsync(documentId))
                .ReturnsAsync(document);

            _customerServiceMock.Setup(s => s.GetCustomerByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _documentApplication.DeleteDocumentAsync(documentId);

            // Assert
            result.Success.Should().BeTrue();
            _documentServiceMock.Verify(s => s.DeleteDocumentAsync(documentId), Times.Once);
            _customerServiceMock.Verify(s => s.UpdateCustomerAsync(customer), Times.Once);
        }

        [Fact]
        public async Task GetDocumentsByCustomerIdAsync_ShouldReturnList()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var documents = new List<Document>
            {
                new Document("Doc1", customerId, DocumentType.Identity),
                new Document("Doc2", customerId, DocumentType.AddressProof)
            };

            _documentServiceMock.Setup(s => s.GetDocumentsByCustomerIdAsync(customerId))
                .ReturnsAsync(documents);

            // Act
            var result = await _documentApplication.GetDocumentsByCustomerIdAsync(customerId);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }
    }
}
