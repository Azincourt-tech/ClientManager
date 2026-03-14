using ClientManager.Domain.Enums;

namespace ClientManager.Application.Dtos.Customer
{
    public class CustomerDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public AddressDto Address { get; set; } = null!;
        public string Document { get; set; } = null!; 
        public CustomerType Type { get; set; }
        public CustomerStatus Status { get; set; }
        public IEnumerable<Document.DocumentDto> Documents { get; set; } = new List<Document.DocumentDto>();
    }
}
