using ClientManager.Api.Controllers;
using ClientManager.Api.Results;
using ClientManager.Application.Dtos.Document;
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
    public class DocumentControllerTests
    {
        private readonly Mock<IDocumentApplication> _documentApplicationMock;
        private readonly Mock<IStringLocalizer<SharedResource>> _localizerMock;
        private readonly DocumentController _documentController;

        public DocumentControllerTests()
        {
            _documentApplicationMock = new Mock<IDocumentApplication>();
            _localizerMock = new Mock<IStringLocalizer<SharedResource>>();

            _documentController = new DocumentController(
                _documentApplicationMock.Object,
                _localizerMock.Object
            );
        }

        [Fact]
        public async Task AttachDocument_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var expectedDocumentId = Guid.NewGuid();
            var type = DocumentType.Identity;
            var expiryDate = DateTimeOffset.Now.AddYears(1);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("passport.pdf");
            fileMock.Setup(f => f.Length).Returns(1024);

            var serviceResponse = new ServiceResponse<Guid>(expectedDocumentId);

            _documentApplicationMock.Setup(x => x.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _documentController.AttachDocument(customerId, fileMock.Object, type, expiryDate);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<Guid>>().Subject;
            apiResult.Data.Should().Be(expectedDocumentId);

            _documentApplicationMock.Verify(a => a.AttachDocumentAsync(customerId, fileMock.Object, type, expiryDate), Times.Once);
        }

        [Fact]
        public async Task GetAttachDocument_WhenExists_ShouldReturnDocument()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var responseDto = new DocumentAttachmentResponseDto
            {
                ContentType = "application/pdf",
                ContentBase64 = "SGVsbG8gV29ybGQ=",
                Info = new DocumentDto
                {
                    Id = documentId,
                    Name = "Identity_Document.pdf",
                    Type = DocumentType.Identity,
                    CreateDate = DateTimeOffset.Now.AddDays(-1),
                    ExpiryDate = DateTimeOffset.Now.AddYears(1)
                }
            };
            var serviceResponse = new ServiceResponse<DocumentAttachmentResponseDto?>(responseDto);

            _documentApplicationMock.Setup(x => x.GetAttachDocumentAsync(documentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _documentController.GetAttachDocument(documentId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<DocumentAttachmentResponseDto>>().Subject;
            apiResult.Data.Should().NotBeNull();
            apiResult.Data!.Should().Be(responseDto);
            apiResult.Data.Info.Name.Should().Be("Identity_Document.pdf");
            apiResult.Data.ContentType.Should().Be("application/pdf");

            _documentApplicationMock.Verify(a => a.GetAttachDocumentAsync(documentId), Times.Once);
        }

        [Fact]
        public async Task UpdateDocument_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var updateDto = new UpdateDocumentDto
            {
                Type = DocumentType.AddressProof,
                ExpiryDate = DateTimeOffset.Now.AddYears(2)
            };
            var serviceResponse = new ServiceResponse<string>("Updated successfully");

            _documentApplicationMock.Setup(x => x.UpdateDocumentAsync(documentId, updateDto))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _documentController.UpdateDocument(documentId, updateDto);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be("Updated successfully");

            _documentApplicationMock.Verify(a => a.UpdateDocumentAsync(documentId, It.Is<UpdateDocumentDto>(d =>
                d.Type == updateDto.Type &&
                d.ExpiryDate == updateDto.ExpiryDate)), Times.Once);
        }

        [Fact]
        public async Task DeleteDocument_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var documentId = Guid.NewGuid();
            var serviceResponse = new ServiceResponse<string>("Deleted");

            _documentApplicationMock.Setup(x => x.DeleteDocumentAsync(documentId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _documentController.DeleteDocument(documentId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<string>>().Subject;
            apiResult.Data.Should().Be("Deleted");

            _documentApplicationMock.Verify(a => a.DeleteDocumentAsync(documentId), Times.Once);
        }

        [Fact]
        public async Task GetDocumentsByCustomerId_WhenSuccessful_ShouldReturnDocuments()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var documents = new List<DocumentDto>
            {
                new DocumentDto { Id = Guid.NewGuid(), Name = "Doc1.pdf", Type = DocumentType.Identity },
                new DocumentDto { Id = Guid.NewGuid(), Name = "Doc2.pdf", Type = DocumentType.AddressProof }
            };
            var serviceResponse = new ServiceResponse<IEnumerable<DocumentDto>>(documents);

            _documentApplicationMock.Setup(x => x.GetDocumentsByCustomerIdAsync(customerId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _documentController.GetDocumentsByCustomerId(customerId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<IEnumerable<DocumentDto>>>().Subject;
            apiResult.Data.Should().HaveCount(2);
            apiResult.Data.Should().BeEquivalentTo(documents);

            _documentApplicationMock.Verify(a => a.GetDocumentsByCustomerIdAsync(customerId), Times.Once);
        }

        [Fact]
        public async Task GetDocumentCountByCustomerId_WhenSuccessful_ShouldReturnCount()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var expectedCount = 5;
            var serviceResponse = new ServiceResponse<int>(expectedCount);

            _documentApplicationMock.Setup(x => x.GetDocumentCountByCustomerIdAsync(customerId))
                .ReturnsAsync(serviceResponse);

            // Act
            var result = await _documentController.GetDocumentCountByCustomerId(customerId);

            // Assert
            result.Should().NotBeNull();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var apiResult = okResult.Value.Should().BeOfType<ApiOkResult<int>>().Subject;
            apiResult.Data.Should().Be(expectedCount);

            _documentApplicationMock.Verify(a => a.GetDocumentCountByCustomerIdAsync(customerId), Times.Once);
        }
    }
}
