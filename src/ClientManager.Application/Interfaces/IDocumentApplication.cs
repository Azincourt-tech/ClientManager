using ClientManager.Domain.Enums;
using ClientManager.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;

namespace ClientManager.Application.Interfaces;

public interface IDocumentApplication
{
    Task<ServiceResponse<Guid>> AttachDocumentAsync(Guid customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null);
    Task<ServiceResponse<AttachmentResult?>> GetAttachDocumentAsync(Guid documentId);
    Task<ServiceResponse<string>> DeleteDocumentAsync(Guid documentId);
    Task<ServiceResponse<int>> GetDocumentCountByCustomerIdAsync(Guid customerId);
}
