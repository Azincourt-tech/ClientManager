using Microsoft.AspNetCore.Http;
using Raven.Client.Documents.Operations.Attachments;

using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Infrastructure.Data.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly IDocumentStore _documentStore;

    public DocumentRepository(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task<string> AttachDocumentAsync(string customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        using var session = _documentStore.OpenAsyncSession();
        
        // Find existing or create new doc metadata for this customer, filename and type
        var document = await session.Query<Document>()
                                .FirstOrDefaultAsync(d => d.Name == file.FileName && d.CustomerId == customerId && d.Type == type).ConfigureAwait(false);

        if (document == null)
        {
            document = new Document(file.FileName, customerId, type, expiryDate);
        }
        else
        {
            document.UpdateExpiryDate(expiryDate);
        }

        await session.StoreAsync(document).ConfigureAwait(false);
        await using var stream = file.OpenReadStream();
        session.Advanced.Attachments.Store(document.Id, document.Name, stream, file.ContentType);
        await session.SaveChangesAsync().ConfigureAwait(false);
        
        return document.Id;
    }

    public async Task<AttachmentResult?> GetAttachDocumentAsync(string documentId)
    {
        using var session = _documentStore.OpenAsyncSession();
        var document = await session.LoadAsync<Document>(documentId).ConfigureAwait(false);

        if (document is null) return null;
        
        var attachment = await session.Advanced.Attachments.GetAsync(document.Id, document.Name).ConfigureAwait(false);
        return attachment;
    }

    public async Task DeleteDocumentAsync(string documentId)
    {
        using var session = _documentStore.OpenAsyncSession();
        var document = await session.LoadAsync<Document>(documentId).ConfigureAwait(false);
        if (document != null)
        {
            session.Delete(document); // This will also delete related attachments in RavenDB 
            await session.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task<int> GetDocumentCountByCustomerIdAsync(string customerId)
    {
        using var session = _documentStore.OpenAsyncSession();
        return await session.Query<Document>().CountAsync(d => d.CustomerId == customerId).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(string customerId)
    {
        using var session = _documentStore.OpenAsyncSession();
        return await session.Query<Document>().Where(d => d.CustomerId == customerId).ToListAsync().ConfigureAwait(false);
    }
}