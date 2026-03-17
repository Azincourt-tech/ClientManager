using ClientManager.Domain.Enums;

namespace ClientManager.Domain.Core.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<Guid> AttachDocumentAsync(Guid customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null);
    Task<Document?> GetDocumentByIdAsync(Guid documentId);
    Task UpdateDocumentAsync(Document document);
    Task<AttachmentResult?> GetAttachDocumentAsync(Guid documentId);
    Task DeleteDocumentAsync(Guid documentId);
    Task<int> GetDocumentCountByCustomerIdAsync(Guid customerId);
    Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(Guid customerId);
}
