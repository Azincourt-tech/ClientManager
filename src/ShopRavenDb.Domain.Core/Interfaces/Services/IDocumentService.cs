using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Domain.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<string> AttachDocumentAsync(string customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null);
    Task<AttachmentResult?> GetAttachDocumentAsync(string documentId);
    Task DeleteDocumentAsync(string documentId);
    Task<int> GetDocumentCountByCustomerIdAsync(string customerId);
    Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(string customerId);
}