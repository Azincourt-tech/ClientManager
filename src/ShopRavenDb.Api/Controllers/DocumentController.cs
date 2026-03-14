using ShopRavenDb.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;
using ShopRavenDb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ShopRavenDb.Api.Controllers;

/// <summary>
/// Controller for managing file attachments associated with entities.
/// </summary>
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
    /// Attaches a file to a specific customer.
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer.</param>
    /// <param name="file">The file to be uploaded.</param>
    /// <returns>A service response containing the document ID of the attached file.</returns>
    [HttpPost("attach/{customerId}", Name = "attach-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AttachDocument(string customerId, IFormFile file)
    {
       var formattedCustomerId = Uri.UnescapeDataString(customerId);
       var response = await _documentApplication.AttachDocumentAsync(formattedCustomerId, file).ConfigureAwait(false);
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
    public async Task<IActionResult> GetAttachDocument(string documentId)
    {
        var formattedDocumentId = Uri.UnescapeDataString(documentId);
        var response = await _documentApplication.GetAttachDocumentAsync(formattedDocumentId).ConfigureAwait(false);

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
    public async Task<IActionResult> DeleteDocument(string documentId)
    {
        var formattedDocumentId = Uri.UnescapeDataString(documentId);
        var response = await _documentApplication.DeleteDocumentAsync(formattedDocumentId).ConfigureAwait(false);
        return response.Success ? Ok(response) : NotFound(response);
    }
}