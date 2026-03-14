namespace ShopRavenDb.Domain.Core.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<string> AttachDocumentAsync(string customerId, IFormFile file);
    Task<AttachmentResult?> GetAttachDocumentAsync(string documentId);
    Task DeleteDocumentAsync(string documentId);
    Task<int> GetDocumentCountByCustomerIdAsync(string customerId);
    Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(string customerId);
}