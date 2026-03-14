using ClientManager.Application.Dtos.Document;
using ClientManager.Domain.Model;

namespace ClientManager.Application.Mappers
{
    public static class DocumentMapper
    {
        public static DocumentDto ToDto(this Document document)
        {
            if (document == null) return null!;

            return new DocumentDto
            {
                Id = document.Id,
                Name = document.Name,
                Type = document.Type,
                CreateDate = document.CreateDate,
                ExpiryDate = document.ExpiryDate
            };
        }
    }
}
