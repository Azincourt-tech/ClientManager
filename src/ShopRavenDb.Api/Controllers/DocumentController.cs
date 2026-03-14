using ShopRavenDb.Domain.Enums;
using ShopRavenDb.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;
using ShopRavenDb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ShopRavenDb.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentApplication _documentApplication;

    public DocumentController(IDocumentApplication documentApplication)
    {
        _documentApplication = documentApplication;
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
       return response.Success ? Ok(response) : BadRequest(response);
    }
    
    /// <summary>
    /// Retrieves an attached file by its unique document ID.
    /// </summary>
    /// <param name="documentId">The ID of the document to retrieve (URL encoded).</param>
    /// <returns>The file stream for download.</returns>
    [HttpGet("get-attach/{documentId}", Name = "get-attach-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<AttachmentResult?>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttachDocument(Guid documentId)
    {
        var response = await _documentApplication.GetAttachDocumentAsync(documentId).ConfigureAwait(false);

        if (!response.Success || response.Data == null)
            return NotFound(response);

        return File(response.Data.Stream, response.Data.Details.ContentType);
    }

    /// <summary>
    /// Removes a document and its associated file from the system.
    /// </summary>
    /// <param name="documentId">The unique identifier of the document to be removed.</param>
    /// <returns>A service response confirming the removal.</returns>
    [HttpDelete("{documentId}", Name = "delete-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var response = await _documentApplication.DeleteDocumentAsync(documentId).ConfigureAwait(false);
        return response.Success ? Ok(response) : NotFound(response);
    }
}