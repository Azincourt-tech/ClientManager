using ClientManager.Domain.Core.Events;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ClientManager.Api.Consumers;

public class DocumentUploadedConsumer(
    IDocumentService documentService,
    ICustomerService customerService,
    ILogger<DocumentUploadedConsumer> logger)
{
    public async Task HandleAsync(DocumentUploadedEvent @event)
    {
        logger.LogInformation("Processing document: {DocumentId} ({FileName}) for customer {CustomerId}", 
            @event.DocumentId, @event.FileName, @event.CustomerId);

        try
        {
            var document = await documentService.GetDocumentByIdAsync(@event.DocumentId);
            if (document == null)
            {
                logger.LogWarning("Document {DocumentId} not found", @event.DocumentId);
                return;
            }

            // 1. File Integrity Check (Simulated)
            await Task.Delay(1000);
            if (@event.FileName.EndsWith(".exe") || @event.FileName.EndsWith(".bat"))
            {
                document.Reject("File integrity check failed: Potentially dangerous file type.");
                await documentService.UpdateDocumentAsync(document);
                logger.LogWarning("Document {DocumentId} rejected due to file type", @event.DocumentId);
                return;
            }

            // 2. OCR Processing (Simulated)
            logger.LogInformation("Running OCR for document {DocumentId}...", @event.DocumentId);
            await Task.Delay(3000);
            
            // Simulation logic: if filename has "invalid" or "error", fail OCR
            if (@event.FileName.Contains("invalid", StringComparison.OrdinalIgnoreCase) || 
                @event.FileName.Contains("error", StringComparison.OrdinalIgnoreCase))
            {
                document.Reject("OCR validation failed: Could not extract data from document image.");
                logger.LogInformation("Document {DocumentId} REJECTED by OCR", @event.DocumentId);
            }
            else
            {
                document.Verify();
                logger.LogInformation("Document {DocumentId} VERIFIED after OCR", @event.DocumentId);
            }

            await documentService.UpdateDocumentAsync(document);

            // 3. Re-evaluate customer status
            var customer = await customerService.GetCustomerByIdAsync(@event.CustomerId);
            if (customer != null)
            {
                var documents = await documentService.GetDocumentsByCustomerIdAsync(@event.CustomerId);
                customer.EvaluateVerificationStatus(documents);
                await customerService.UpdateCustomerAsync(customer);
                
                logger.LogInformation("Customer {CustomerId} status updated to {Status}", 
                    customer.Id, customer.Status);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing document {DocumentId}", @event.DocumentId);
            throw;
        }
    }
}
