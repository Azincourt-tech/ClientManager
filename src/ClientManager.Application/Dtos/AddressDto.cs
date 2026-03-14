namespace ClientManager.Application.Dtos
{
    public class AddressDto
    {
        public string Street { get; set; } = null!;
        public int Number { get; set; }
        public string Complement { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
    }
}

