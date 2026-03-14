namespace ShopRavenDb.Domain.Model
{
    public class Customer
    {
        public string Id { get; private set; } = null!;
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime BirthDate { get; private set; }
        public Address? Address { get; private set; }
        public string Cpf { get; private set; }
        public bool IsActive { get; private set; }

        // Construtor vazio para o RavenDB (Desserialização)
        private Customer() { }

        public Customer(string name, string email, DateTime birthDate, string cpf, Address? address = null)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Nome invalido", nameof(name)) : name;
            Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email invalido", nameof(email)) : email;
            BirthDate = birthDate;
            Cpf = string.IsNullOrWhiteSpace(cpf) ? throw new ArgumentException("CPF invalido", nameof(cpf)) : cpf;
            Address = address;
            Activate();
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactive()
        {
            IsActive = false;
        }

        public void UpdateDetails(string name, string email, string cpf, Address? address)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Nome invalido", nameof(name)) : name;
            Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email invalido", nameof(email)) : email;
            Cpf = string.IsNullOrWhiteSpace(cpf) ? throw new ArgumentException("CPF invalido", nameof(cpf)) : cpf;
            Address = address;
        }
    }
}
