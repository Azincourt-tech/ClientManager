using FluentValidation;
using Microsoft.AspNetCore.Http;
using ShopRavenDb.Application.Interfaces;
using ShopRavenDb.Domain.Core.Interfaces.Services;
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

    public async Task<string> AttachDocumentAsync(IFormFile file)
    {
        await _validator.ValidateAndThrowAsync(file);
        return await _documentService.AttachDocumentAsync(file).ConfigureAwait(false);
    }

    public async Task<AttachmentResult?> GetAttachDocumentAsync(string documentId)
    {
        return await _documentService.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
    }
}