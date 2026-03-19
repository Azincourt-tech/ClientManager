using ClientManager.Domain.Enums;
using ClientManager.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;

using ClientManager.Application.Dtos.Document;

namespace ClientManager.Application.Interfaces;

public interface IDocumentApplication
{
    Task<ServiceResponse<Guid>> AttachDocumentAsync(Guid customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null);
    Task<ServiceResponse<DocumentAttachmentResponseDto?>> GetAttachDocumentAsync(Guid documentId);
    Task<ServiceResponse<string>> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto updateDocumentDto);
    Task<ServiceResponse<string>> DeleteDocumentAsync(Guid documentId);
    Task<ServiceResponse<IEnumerable<DocumentDto>>> GetDocumentsByCustomerIdAsync(Guid customerId);
    Task<ServiceResponse<int>> GetDocumentCountByCustomerIdAsync(Guid customerId);
}
