namespace ShopRavenDb.Domain.Core.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<string> AttachDocumentAsync(IFormFile file);
    Task<AttachmentResult?> GetAttachDocumentAsync(string documentId);
}