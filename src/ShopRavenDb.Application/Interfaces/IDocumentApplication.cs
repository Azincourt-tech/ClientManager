using ShopRavenDb.Domain.Core.Responses;
using Raven.Client.Documents.Operations.Attachments;
using Microsoft.AspNetCore.Http;

namespace ShopRavenDb.Application.Interfaces;

public interface IDocumentApplication
{
    Task<ServiceResponse<string>> AttachDocumentAsync(IFormFile file);
    Task<ServiceResponse<AttachmentResult?>> GetAttachDocumentAsync(string documentId);
}