namespace ShopRavenDb.Application.Interfaces;

public interface IDocumentApplication
{
    Task<string> AttachDocumentAsync(IFormFile file);
    Task<AttachmentResult?> GetAttachDocumentAsync(string documentId);
}