using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Domain.Model;

public class Document
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string CustomerId { get; private set; } = null!;
    public DocumentType Type { get; private set; }
    public DateTimeOffset CreateDate { get; private set; }
    public DateTimeOffset? ExpiryDate { get; private set; }

    private Document() { }

    public Document(string name, string customerId, DocumentType type, DateTimeOffset? expiryDate = null)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name invalid", nameof(name)) : name;
        CustomerId = string.IsNullOrWhiteSpace(customerId) ? throw new ArgumentException("CustomerId invalid", nameof(customerId)) : customerId;
        Type = type;
        CreateDate = DateTimeOffset.UtcNow;
        ExpiryDate = expiryDate;
    }

    public void UpdateExpiryDate(DateTimeOffset? expiryDate) => ExpiryDate = expiryDate;

    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTimeOffset.UtcNow;
}