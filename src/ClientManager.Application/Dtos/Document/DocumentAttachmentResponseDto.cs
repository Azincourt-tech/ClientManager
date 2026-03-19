namespace ClientManager.Application.Dtos.Document;

public class DocumentAttachmentResponseDto
{
    public DocumentDto Info { get; set; } = null!;
    public string ContentBase64 { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
