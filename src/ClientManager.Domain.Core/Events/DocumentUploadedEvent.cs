using ClientManager.Domain.Enums;

namespace ClientManager.Domain.Core.Events;

public record DocumentUploadedEvent(
    Guid DocumentId,
    Guid CustomerId,
    DocumentType Type,
    string FileName,
    DateTime CreatedAt);
