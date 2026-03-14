using Raven.Client.Documents.Operations.Attachments;

namespace ShopRavenDb.Domain.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<string> AttachDocumentAsync(IFormFile file)
    {
        return await _documentRepository.AttachDocumentAsync(file).ConfigureAwait(false);
    }

    public async Task<AttachmentResult?> GetAttachDocumentAsync(string documentId)
    {
        return await _documentRepository.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
    }
}