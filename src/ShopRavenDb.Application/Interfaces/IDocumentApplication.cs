using ShopRavenDb.Domain.Enums;
using ShopRavenDb.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;

namespace ShopRavenDb.Application.Interfaces;

public interface IDocumentApplication
{
    Task<ServiceResponse<string>> AttachDocumentAsync(string customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null);
    Task<ServiceResponse<AttachmentResult?>> GetAttachDocumentAsync(string documentId);
    Task<ServiceResponse<string>> DeleteDocumentAsync(string documentId);
    Task<ServiceResponse<int>> GetDocumentCountByCustomerIdAsync(string customerId);
}