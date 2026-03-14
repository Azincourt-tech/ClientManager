using FluentValidation;
using Microsoft.AspNetCore.Http;
using ShopRavenDb.Application.Interfaces;
using ShopRavenDb.Domain.Core.Interfaces.Services;
using ShopRavenDb.Domain.Core.Responses;
using ShopRavenDb.Domain.Enums;
using Raven.Client.Documents.Operations.Attachments;

namespace ShopRavenDb.Application;

public class DocumentApplication : IDocumentApplication
{
    private readonly IDocumentService _documentService;
    private readonly IFileValidator _fileValidator;
    private readonly IValidator<IFormFile> _fluentValidator;

    public DocumentApplication(IDocumentService documentService, IFileValidator fileValidator, IValidator<IFormFile> fluentValidator)
    {
        _documentService = documentService;
        _fileValidator = fileValidator;
        _fluentValidator = fluentValidator;
    }

    public async Task<ServiceResponse<string>> AttachDocumentAsync(string customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        await _fluentValidator.ValidateAndThrowAsync(file);
        
        if (!_fileValidator.IsValid(file, out string errorMessage))
        {
            return ServiceResponse<string>.Fail(errorMessage);
        }

        var res = await _documentService.AttachDocumentAsync(customerId, file, type, expiryDate).ConfigureAwait(false);
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