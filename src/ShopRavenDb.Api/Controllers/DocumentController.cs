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
    /// Attaches a file to a document record.
    /// </summary>
    /// <param name="file">The file to be uploaded.</param>
    /// <returns>A service response containing the document ID of the attached file.</returns>
    [HttpPost("attach", Name = "attach-document")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ServiceResponse<string>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AttachDocument(IFormFile file)
    {
       var response = await _documentApplication.AttachDocumentAsync(file).ConfigureAwait(false);
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
}