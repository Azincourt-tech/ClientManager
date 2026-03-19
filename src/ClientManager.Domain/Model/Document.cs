using ClientManager.Domain.Enums;

namespace ClientManager.Domain.Model;

public class Document
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public DocumentType Type { get; private set; }
    public DateTimeOffset CreateDate { get; private set; }
    public DateTimeOffset? ExpiryDate { get; private set; }
    public bool IsDeleted { get; private set; }

    public DocumentStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    private Document() { }

    public Document(string name, Guid customerId, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        CustomerId = customerId;
        Type = type;
        CreateDate = DateTimeOffset.UtcNow;
        ExpiryDate = expiryDate;
        Status = DocumentStatus.Pending;
    }

    public void Verify()
    {
        Status = DocumentStatus.Verified;
        RejectionReason = null;
    }

    public void Reject(string reason)
    {
        Status = DocumentStatus.Rejected;
        RejectionReason = reason;
    }

    public void UpdateExpiryDate(DateTimeOffset? expiryDate) => ExpiryDate = expiryDate;

    public void UpdateType(DocumentType type) => Type = type;

    public void Delete() => IsDeleted = true;

    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTimeOffset.UtcNow;
}
