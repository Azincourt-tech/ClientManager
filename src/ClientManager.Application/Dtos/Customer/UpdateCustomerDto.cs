namespace ClientManager.Application.Dtos.Customer
{
    public class UpdateCustomerDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Document { get; set; } = null!;
        public AddressDto Address { get; set; } = null!;
    }
}
