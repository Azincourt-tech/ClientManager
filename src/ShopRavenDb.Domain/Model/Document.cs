namespace ShopRavenDb.Domain.Model;

public class Document
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; }
    public string CustomerId { get; private set; }
    public DateTimeOffset CreateDate { get; private set; }

    private Document() { }

    public Document(string name, string customerId)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name invalid", nameof(name)) : name;
        CustomerId = string.IsNullOrWhiteSpace(customerId) ? throw new ArgumentException("CustomerId invalid", nameof(customerId)) : customerId;
        CreateDate = DateTimeOffset.UtcNow;
    }
}