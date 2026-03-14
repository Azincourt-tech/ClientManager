using FluentValidation;
using Microsoft.AspNetCore.Http;
using ClientManager.Application.Interfaces;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Core.Responses;
using ClientManager.Domain.Enums;
using Raven.Client.Documents.Operations.Attachments;

namespace ClientManager.Application;

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

    public async Task<ServiceResponse<Guid>> AttachDocumentAsync(Guid customerId, IFormFile file, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        await _fluentValidator.ValidateAndThrowAsync(file);
        
        if (!_fileValidator.IsValid(file, out string errorMessage))
        {
            return ServiceResponse<Guid>.Fail(errorMessage);
        }

        var res = await _documentService.AttachDocumentAsync(customerId, file, type, expiryDate).ConfigureAwait(false);
        return ServiceResponse<Guid>.Ok(res, "DocumentAttached");
    }

    public async Task<ServiceResponse<AttachmentResult?>> GetAttachDocumentAsync(Guid documentId)
    {
        var res = await _documentService.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
        if (res == null)
            return ServiceResponse<AttachmentResult?>.Fail("DocumentNotFound");
            
        return ServiceResponse<AttachmentResult?>.Ok(res);
    }

    public async Task<ServiceResponse<string>> DeleteDocumentAsync(Guid documentId)
    {
        await _documentService.DeleteDocumentAsync(documentId).ConfigureAwait(false);
        return ServiceResponse<string>.Ok(documentId.ToString(), "DocumentRemoved");
    }

    public async Task<ServiceResponse<int>> GetDocumentCountByCustomerIdAsync(Guid customerId)
    {
        var count = await _documentService.GetDocumentCountByCustomerIdAsync(customerId).ConfigureAwait(false);
        return ServiceResponse<int>.Ok(count);
    }
}
