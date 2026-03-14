using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Application.Dtos
{
    public class CustomerDto
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public AddressDto Address { get; set; }
        public string Cpf { get; set; } // Also used for CNPJ
        public CustomerType Type { get; set; }
        public CustomerStatus Status { get; set; }
    }
}
