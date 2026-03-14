namespace ShopRavenDb.Domain.Model
{
    public class Address
    {
        public string Street { get; private set; }
        public int Number { get; private set; }
        public string Complement { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string PostalCode { get; private set; }
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
