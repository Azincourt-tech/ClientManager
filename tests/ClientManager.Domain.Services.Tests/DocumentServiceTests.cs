using ClientManager.Domain.Core.Interfaces.Repositories;
using ClientManager.Domain.Enums;
using ClientManager.Domain.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Raven.Client.Documents.Operations.Attachments;
using Xunit;

namespace ClientManager.Domain.Services.Tests
{
    public class DocumentServiceTests
    {
        private readonly Mock<IDocumentRepository> _documentRepositoryMock;
        private readonly DocumentService _documentService;

        public DocumentServiceTests()
        {
            _documentRepositoryMock = new Mock<IDocumentRepository>();
            _documentService = new DocumentService(_documentRepositoryMock.Object);
        }

        [Fact]
        public async Task AttachDocumentAsync_ShouldCallRepository()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            var type = DocumentType.Identity;
            var expiryDate = DateTimeOffset.Now.AddYears(1);
            var expectedId = Guid.NewGuid();

            _documentRepositoryMock.Setup(r => r.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _documentService.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate);

            // Assert
            result.Should().Be(expectedId);
            _documentRepositoryMock.Verify(r => r.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate), Times.Once);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_ShouldReturnFromRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            var document = new Document("Test", Guid.NewGuid(), DocumentType.Identity);
            _documentRepositoryMock.Setup(r => r.GetDocumentByIdAsync(id)).ReturnsAsync(document);

            // Act
            var result = await _documentService.GetDocumentByIdAsync(id);

            // Assert
            result.Should().Be(document);
            _documentRepositoryMock.Verify(r => r.GetDocumentByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task UpdateDocumentAsync_ShouldCallRepository()
        {
            // Arrange
            var document = new Document("Test", Guid.NewGuid(), DocumentType.Identity);

            // Act
            await _documentService.UpdateDocumentAsync(document);

            // Assert
            _documentRepositoryMock.Verify(r => r.UpdateDocumentAsync(document), Times.Once);
        }

        [Fact]
        public async Task GetAttachDocumentAsync_ShouldReturnFromRepository()
        {
            // Arrange
            var id = Guid.NewGuid();
            // AttachmentResult has a private constructor or is complex to mock, we just mock the return
            var attachmentResult = (AttachmentResult)null!;
            _documentRepositoryMock.Setup(r => r.GetAttachDocumentAsync(id)).ReturnsAsync(attachmentResult);

            // Act
            var result = await _documentService.GetAttachDocumentAsync(id);

            // Assert
            result.Should().Be(attachmentResult);
            _documentRepositoryMock.Verify(r => r.GetAttachDocumentAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteDocumentAsync_ShouldCallRepository()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            await _documentService.DeleteDocumentAsync(id);

            // Assert
            _documentRepositoryMock.Verify(r => r.DeleteDocumentAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetDocumentCountByCustomerIdAsync_ShouldReturnFromRepository()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var count = 5;
            _documentRepositoryMock.Setup(r => r.GetDocumentCountByCustomerIdAsync(customerId)).ReturnsAsync(count);

            // Act
            var result = await _documentService.GetDocumentCountByCustomerIdAsync(customerId);

            // Assert
            result.Should().Be(count);
            _documentRepositoryMock.Verify(r => r.GetDocumentCountByCustomerIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task GetDocumentsByCustomerIdAsync_ShouldReturnFromRepository()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var documents = new List<Document> { new Document("Test", customerId, DocumentType.Identity) };
            _documentRepositoryMock.Setup(r => r.GetDocumentsByCustomerIdAsync(customerId)).ReturnsAsync(documents);

            // Act
            var result = await _documentService.GetDocumentsByCustomerIdAsync(customerId);

            // Assert
            result.Should().BeEquivalentTo(documents);
            _documentRepositoryMock.Verify(r => r.GetDocumentsByCustomerIdAsync(customerId), Times.Once);
        }
    }
}
