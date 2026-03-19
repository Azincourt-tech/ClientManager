using ClientManager.Api.Results;
using ClientManager.Domain.Enums;
using Microsoft.Extensions.Localization;

namespace ClientManager.Api.Controllers;

[Route("api/[controller]")]
public class DocumentController : MainController
{
    private readonly IDocumentApplication _documentApplication;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public DocumentController(IDocumentApplication documentApplication, IStringLocalizer<SharedResource> localizer)
    {
        _documentApplication = documentApplication;
        _localizer = localizer;
    }

    /// <summary>
    /// Attaches a file to a specific customer with categorization and optional expiry date.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="file">The file to be uploaded.</param>
    /// <param name="type">The type of document (e.g., Identity, AddressProof).</param>
    /// <param name="expiryDate">The optional expiry date of the document.</param>
    /// <returns>A service response containing the document ID of the attached file.</returns>
    [HttpPost("attach/{customerId}", Name = "attach-document")]
    [EndpointSummary("Attaches a file to a specific customer with categorization and optional expiry date.")]
    [ProducesResponseType(typeof(ApiOkResult<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AttachDocument(Guid customerId, IFormFile file, [FromQuery] DocumentType type, [FromQuery] DateTimeOffset? expiryDate = null)
    {
        var response = await _documentApplication.AttachDocumentAsync(customerId, file, type, expiryDate).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Retrieves an attached file by its unique document ID, including document metadata and base64 content.
    /// </summary>
    /// <param name="documentId">The ID of the document to retrieve (URL encoded).</param>
    /// <returns>A service response containing document information and its file in base64.</returns>
    [HttpGet("get-attach/{documentId}", Name = "get-attach-document")]
    [EndpointSummary("Retrieves an attached file by its unique document ID, including document metadata and base64 content.")]
    [ProducesResponseType(typeof(ApiOkResult<DocumentAttachmentResponseDto?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAttachDocument(Guid documentId)
    {
        var response = await _documentApplication.GetAttachDocumentAsync(documentId).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Updates the categorization and expiry date of an existing document.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="updateDocumentDto">The updated document information.</param>
    /// <returns>A service response containing the ID of the updated document.</returns>
    [HttpPut("{documentId}", Name = "update-document")]
    [EndpointSummary("Updates the categorization and expiry date of an existing document.")]
    [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] UpdateDocumentDto updateDocumentDto)
    {
        var response = await _documentApplication.UpdateDocumentAsync(documentId, updateDocumentDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Removes a document and its associated file from the system.
    /// </summary>    /// <param name="documentId">The unique identifier of the document to be removed.</param>
    /// <returns>A service response confirming the removal.</returns>
    [HttpDelete("{documentId}", Name = "delete-document")]
    [EndpointSummary("Removes a document and its associated file from the system.")]
    [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var response = await _documentApplication.DeleteDocumentAsync(documentId).ConfigureAwait(false);
        return ServiceResponse(response);
    }
}
