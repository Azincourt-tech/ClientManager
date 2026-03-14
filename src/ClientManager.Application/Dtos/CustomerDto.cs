using ClientManager.Domain.Enums;

namespace ClientManager.Application.Dtos
{
    public class CustomerDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public AddressDto Address { get; set; } = null!;
        public string Document { get; set; } = null!; // Also used for CNPJ
        public CustomerType Type { get; set; }
        public CustomerStatus Status { get; set; }
    }
}

