using ClientManager.Domain.Enums;

namespace ClientManager.Application.Dtos.Document;

public class UpdateDocumentDto
{
    public DocumentType Type { get; set; }
    public DateTimeOffset? ExpiryDate { get; set; }
}
