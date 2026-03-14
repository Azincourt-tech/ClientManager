using Raven.Client.Documents.Operations.Attachments;

namespace ShopRavenDb.Domain.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<string> AttachDocumentAsync(string customerId, IFormFile file)
    {
        return await _documentRepository.AttachDocumentAsync(customerId, file).ConfigureAwait(false);
    }

    public async Task<AttachmentResult?> GetAttachDocumentAsync(string documentId)
    {
        return await _documentRepository.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
    }

    public async Task DeleteDocumentAsync(string documentId)
    {
        await _documentRepository.DeleteDocumentAsync(documentId).ConfigureAwait(false);
    }

    public async Task<int> GetDocumentCountByCustomerIdAsync(string customerId)
    {
        return await _documentRepository.GetDocumentCountByCustomerIdAsync(customerId).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(string customerId)
    {
        return await _documentRepository.GetDocumentsByCustomerIdAsync(customerId).ConfigureAwait(false);
    }
}