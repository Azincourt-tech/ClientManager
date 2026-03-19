using ClientManager.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Raven.Client.Documents.Operations.Attachments;
using Raven.Client.Documents.Session;

namespace ClientManager.Infrastructure.Data.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly IAsyncDocumentSession _session;

    public DocumentRepository(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<Guid> AttachDocumentAsync(Guid customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        // Find existing or create new doc metadata for this customer, filename and type
        var document = await _session.Query<Document>()
                                .FirstOrDefaultAsync(d => d.Name == file.FileName && d.CustomerId == customerId && d.Type == type).ConfigureAwait(false);

        if (document == null)
        {
            document = new Document(file.FileName, customerId, type, expiryDate);
        }
        else
        {
            document.UpdateExpiryDate(expiryDate);
        }

        // Passamos o ID explicitamente como string
        await _session.StoreAsync(document, document.Id.ToString()).ConfigureAwait(false);
        await using var stream = file.OpenReadStream();
        _session.Advanced.Attachments.Store(document.Id.ToString(), document.Name, stream, file.ContentType);
        await _session.SaveChangesAsync().ConfigureAwait(false);

        return document.Id;
    }

    public async Task<Document?> GetDocumentByIdAsync(Guid documentId)
    {
        var document = await _session.LoadAsync<Document>(documentId.ToString()).ConfigureAwait(false);
        return document is not null && !document.IsDeleted ? document : null;
    }

    public async Task UpdateDocumentAsync(Document document)
    {
        await _session.StoreAsync(document, document.Id.ToString()).ConfigureAwait(false);
        await _session.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<AttachmentResult?> GetAttachDocumentAsync(Guid documentId)
    {
        var document = await _session.LoadAsync<Document>(documentId.ToString()).ConfigureAwait(false);

        if (document is null || document.IsDeleted) return null;

        var attachment = await _session.Advanced.Attachments.GetAsync(document.Id.ToString(), document.Name).ConfigureAwait(false);
        return attachment;
    }

    public async Task DeleteDocumentAsync(Guid documentId)
    {
        var document = await _session.LoadAsync<Document>(documentId.ToString()).ConfigureAwait(false);
        if (document != null)
        {
            document.Delete();
            await _session.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    public async Task<int> GetDocumentCountByCustomerIdAsync(Guid customerId)
    {
        return await _session.Query<Document>().CountAsync(d => d.CustomerId == customerId && !d.IsDeleted).ConfigureAwait(false);
    }

    public async Task<IEnumerable<Document>> GetDocumentsByCustomerIdAsync(Guid customerId)
    {
        return await _session.Query<Document>()
                    .Where(d => d.CustomerId == customerId && !d.IsDeleted)
                    .ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Obtém o histórico de revisões (rastreabilidade) dos metadados do documento.
    /// Requer que as Revisions estejam ativadas no RavenDB Studio.
    /// </summary>
    public async Task<IEnumerable<Document>> GetDocumentHistoryAsync(Guid id)
    {
        return await _session.Advanced.Revisions.GetForAsync<Document>(id.ToString()).ConfigureAwait(false);
    }
}
