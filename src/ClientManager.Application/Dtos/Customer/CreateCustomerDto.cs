using ClientManager.Domain.Enums;

namespace ClientManager.Application.Dtos.Customer
{
    public class CreateCustomerDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public AddressDto Address { get; set; } = null!;
        public string Document { get; set; } = null!; 
        public CustomerType Type { get; set; }
    }
}
