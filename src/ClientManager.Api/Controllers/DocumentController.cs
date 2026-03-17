using ClientManager.Domain.Core.Responses;
using ClientManager.Domain.Enums;

namespace ClientManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentApplication _documentApplication;
    private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

    public DocumentController(IDocumentApplication documentApplication, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<Guid>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AttachDocument(Guid customerId, IFormFile file, [FromQuery] DocumentType type, [FromQuery] DateTimeOffset? expiryDate = null)
    {
        var response = await _documentApplication.AttachDocumentAsync(customerId, file, type, expiryDate).ConfigureAwait(false);
        if (!response.Success)
        {
            response.Message = _localizer[response.Message];
            return BadRequest(response);
        }
        response.Message = _localizer[response.Message];
        return Ok(response);
    }

    /// <summary>
    /// Retrieves an attached file by its unique document ID, including document metadata and base64 content.
    /// </summary>
    /// <param name="documentId">The ID of the document to retrieve (URL encoded).</param>
    /// <returns>A service response containing document information and its file in base64.</returns>
    [HttpGet("get-attach/{documentId}", Name = "get-attach-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<DocumentAttachmentResponseDto?>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttachDocument(Guid documentId)
    {
        var response = await _documentApplication.GetAttachDocumentAsync(documentId).ConfigureAwait(false);

        if (!response.Success || response.Data == null)
        {
            response.Message = _localizer[response.Message];
            return NotFound(response);
        }

        response.Message = _localizer[response.Message];
        return Ok(response);
    }

    /// <summary>
    /// Updates the categorization and expiry date of an existing document.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document.</param>
    /// <param name="updateDocumentDto">The updated document information.</param>
    /// <returns>A service response containing the ID of the updated document.</returns>
    [HttpPut("{documentId}", Name = "update-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] UpdateDocumentDto updateDocumentDto)
    {
        var response = await _documentApplication.UpdateDocumentAsync(documentId, updateDocumentDto).ConfigureAwait(false);
        if (!response.Success)
        {
            response.Message = _localizer[response.Message];
            return NotFound(response);
        }
        response.Message = _localizer[response.Message];
        return Ok(response);
    }

    /// <summary>
    /// Removes a document and its associated file from the system.
    /// </summary>    /// <param name="documentId">The unique identifier of the document to be removed.</param>
    /// <returns>A service response confirming the removal.</returns>
    [HttpDelete("{documentId}", Name = "delete-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var response = await _documentApplication.DeleteDocumentAsync(documentId).ConfigureAwait(false);
        if (!response.Success)
        {
            response.Message = _localizer[response.Message];
            return NotFound(response);
        }
        response.Message = _localizer[response.Message];
        return Ok(response);
    }
}
