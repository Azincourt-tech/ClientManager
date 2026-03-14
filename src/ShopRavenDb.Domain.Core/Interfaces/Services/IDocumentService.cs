namespace ShopRavenDb.Domain.Core.Interfaces.Services;

public interface IDocumentService
{
    Task<string> AttachDocumentAsync(IFormFile file);
    Task<AttachmentResult?> GetAttachDocumentAsync(string documentId);
}