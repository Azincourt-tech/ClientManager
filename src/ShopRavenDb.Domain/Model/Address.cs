namespace ShopRavenDb.Domain.Model
{
    public class Address
    {
        public string Street { get; private set; } = null!;
        public int Number { get; private set; }
        public string Complement { get; private set; } = null!;
        public string City { get; private set; } = null!;
        public string State { get; private set; } = null!;
        public string PostalCode { get; private set; } = null!;
        public bool IsActive { get; private set; }

        private Address() { }

        public Address(string street, int number, string complement, string city, string state, string postalCode)
        {
            Street = street;
            Number = number;
            Complement = complement;
            City = city;
            State = state;
            PostalCode = postalCode;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
