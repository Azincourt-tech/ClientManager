namespace ShopRavenDb.Domain.Model;

public class Document
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; }
    public DateTime CreateDate { get; private set; }

    private Document() { }

    public Document(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Name invalid", nameof(name)) : name;
        CreateDate = DateTime.UtcNow;
    }
}