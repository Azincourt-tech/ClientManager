using ClientManager.Application.Mappers;
using ClientManager.Domain.Core.Responses;
using ClientManager.Domain.Enums;
using FluentValidation;

namespace ClientManager.Application;

public class DocumentApplication : IDocumentApplication
{
    private readonly IDocumentService _documentService;
    private readonly ICustomerService _customerService;
    private readonly IFileValidator _fileValidator;
    private readonly IValidator<IFormFile> _fluentValidator;

    public DocumentApplication(
        IDocumentService documentService,
        ICustomerService customerService,
        IFileValidator fileValidator,
        IValidator<IFormFile> fluentValidator)
    {
        _documentService = documentService;
        _customerService = customerService;
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

        // Re-evaluate customer status
        await ReevaluateCustomerStatusAsync(customerId).ConfigureAwait(false);

        return ServiceResponse<Guid>.Ok(res, "DocumentAttached");
    }

    public async Task<ServiceResponse<DocumentAttachmentResponseDto?>> GetAttachDocumentAsync(Guid documentId)
    {
        var doc = await _documentService.GetDocumentByIdAsync(documentId).ConfigureAwait(false);
        if (doc == null)
            return ServiceResponse<DocumentAttachmentResponseDto?>.Fail("DocumentNotFound");

        using var attachment = await _documentService.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
        if (attachment == null)
            return ServiceResponse<DocumentAttachmentResponseDto?>.Fail("AttachmentNotFound");

        using var ms = new MemoryStream();
        await attachment.Stream.CopyToAsync(ms).ConfigureAwait(false);
        var contentBase64 = Convert.ToBase64String(ms.ToArray());

        var res = new DocumentAttachmentResponseDto
        {
            Info = doc.ToDto(),
            ContentBase64 = contentBase64,
            ContentType = attachment.Details.ContentType
        };

        return ServiceResponse<DocumentAttachmentResponseDto?>.Ok(res);
    }

    public async Task<ServiceResponse<string>> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto updateDocumentDto)
    {
        var document = await _documentService.GetDocumentByIdAsync(documentId).ConfigureAwait(false);
        if (document == null)
            return ServiceResponse<string>.Fail("DocumentNotFound");

        document.UpdateType(updateDocumentDto.Type);
        document.UpdateExpiryDate(updateDocumentDto.ExpiryDate);

        await _documentService.UpdateDocumentAsync(document).ConfigureAwait(false);

        // Re-evaluate customer status
        await ReevaluateCustomerStatusAsync(document.CustomerId).ConfigureAwait(false);

        return ServiceResponse<string>.Ok(documentId.ToString(), "DocumentUpdated");
    }

    public async Task<ServiceResponse<string>> DeleteDocumentAsync(Guid documentId)
    {
        var document = await _documentService.GetDocumentByIdAsync(documentId).ConfigureAwait(false);
        if (document == null)
            return ServiceResponse<string>.Fail("DocumentNotFound");

        var customerId = document.CustomerId;

        await _documentService.DeleteDocumentAsync(documentId).ConfigureAwait(false);

        // Re-evaluate customer status
        await ReevaluateCustomerStatusAsync(customerId).ConfigureAwait(false);

        return ServiceResponse<string>.Ok(documentId.ToString(), "DocumentRemoved");
    }

    public async Task<ServiceResponse<IEnumerable<DocumentDto>>> GetDocumentsByCustomerIdAsync(Guid customerId)
    {
        var documents = await _documentService.GetDocumentsByCustomerIdAsync(customerId).ConfigureAwait(false);
        var dtos = documents.Select(x => x.ToDto());
        return ServiceResponse<IEnumerable<DocumentDto>>.Ok(dtos);
    }

    public async Task<ServiceResponse<int>> GetDocumentCountByCustomerIdAsync(Guid customerId)
    {
        var count = await _documentService.GetDocumentCountByCustomerIdAsync(customerId).ConfigureAwait(false);
        return ServiceResponse<int>.Ok(count);
    }

    private async Task ReevaluateCustomerStatusAsync(Guid customerId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId).ConfigureAwait(false);
        if (customer != null)
        {
            var documents = await _documentService.GetDocumentsByCustomerIdAsync(customerId).ConfigureAwait(false);
            customer.EvaluateVerificationStatus(documents);
            await _customerService.UpdateCustomerAsync(customer).ConfigureAwait(false);
        }
    }
}
