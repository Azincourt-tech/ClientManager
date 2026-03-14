using Raven.Client.Documents.Operations.Attachments;

using ClientManager.Domain.Enums;

namespace ClientManager.Domain.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _documentRepository;

    public DocumentService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<Guid> AttachDocumentAsync(Guid customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        return await _documentRepository.AttachDocumentAsync(customerId, file, type, expiryDate).ConfigureAwait(false);
    }

    public async Task<AttachmentResult?> GetAttachDocumentAsync(Guid documentId)
    {
        return await _documentRepository.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
    }

    public async Task DeleteDocumentAsync(Guid documentId)
    {
        await _documentRepository.DeleteDocumentAsync(documentId).ConfigureAwait(false);
    }

    public async Task<int> GetDocumentCountByCustomerIdAsync(Guid customerId)
    {
        return await _documentRepository.GetDocumentCountByCustomerIdAsync(customerId).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(Guid customerId)
    {
        return await _documentRepository.GetDocumentsByCustomerIdAsync(customerId).ConfigureAwait(false);
    }
}
