using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Domain.Model;

public class Document
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid CustomerId { get; private set; }
    public DocumentType Type { get; private set; }
    public DateTimeOffset CreateDate { get; private set; }
    public DateTimeOffset? ExpiryDate { get; private set; }

    private Document() { }

    public Document(string name, Guid customerId, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        Id = Guid.NewGuid();
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name invalid", nameof(name)) : name;
        CustomerId = customerId;
        Type = type;
        CreateDate = DateTimeOffset.UtcNow;
        ExpiryDate = expiryDate;
    }

    public void UpdateExpiryDate(DateTimeOffset? expiryDate) => ExpiryDate = expiryDate;

    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTimeOffset.UtcNow;
}