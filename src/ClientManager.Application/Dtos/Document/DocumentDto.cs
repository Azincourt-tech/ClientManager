using ClientManager.Domain.Enums;

namespace ClientManager.Application.Dtos.Document
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DocumentType Type { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
    }
}
