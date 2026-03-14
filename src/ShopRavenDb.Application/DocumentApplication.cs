using FluentValidation;
using Microsoft.AspNetCore.Http;
using ShopRavenDb.Application.Interfaces;
using ShopRavenDb.Domain.Core.Interfaces.Services;
using ShopRavenDb.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;

namespace ShopRavenDb.Application;

public class DocumentApplication : IDocumentApplication
{
    private readonly IDocumentService _documentService;
    private readonly IValidator<IFormFile> _validator;

    public DocumentApplication(IDocumentService documentService, IValidator<IFormFile> validator)
    {
        _documentService = documentService;
        _validator = validator;
    }

    public async Task<ServiceResponse<string>> AttachDocumentAsync(string customerId, IFormFile file)
    {
        await _validator.ValidateAndThrowAsync(file);
        var res = await _documentService.AttachDocumentAsync(customerId, file).ConfigureAwait(false);
        return ServiceResponse<string>.Ok(res, "Document successfully attached!");
    }

    public async Task<ServiceResponse<AttachmentResult?>> GetAttachDocumentAsync(string documentId)
    {
        var res = await _documentService.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
        if (res == null)
            return ServiceResponse<AttachmentResult?>.Fail("Document not found.");
            
        return ServiceResponse<AttachmentResult?>.Ok(res);
    }

    public async Task<ServiceResponse<string>> DeleteDocumentAsync(string documentId)
    {
        await _documentService.DeleteDocumentAsync(documentId).ConfigureAwait(false);
        return ServiceResponse<string>.Ok(documentId, "Document successfully removed!");
    }

    public async Task<ServiceResponse<int>> GetDocumentCountByCustomerIdAsync(string customerId)
    {
        var count = await _documentService.GetDocumentCountByCustomerIdAsync(customerId).ConfigureAwait(false);
        return ServiceResponse<int>.Ok(count);
    }
}