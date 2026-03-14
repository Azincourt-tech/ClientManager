using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Domain.Model
{
    public class Customer
    {
        public string Id { get; private set; } = null!;
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTimeOffset BirthDate { get; private set; }
        public Address? Address { get; private set; }
        public string Cpf { get; private set; } // Can be CNPJ if LegalEntity
        public CustomerType Type { get; private set; }
        public CustomerStatus Status { get; private set; }

        // Construtor vazio para o RavenDB (Desserialização)
        private Customer() { }

        public Customer(string name, string email, DateTimeOffset birthDate, string cpf, CustomerType type, Address? address = null)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Nome invalido", nameof(name)) : name;
            Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email invalido", nameof(email)) : email;
            BirthDate = birthDate;
            Cpf = string.IsNullOrWhiteSpace(cpf) ? throw new ArgumentException("CPF/CNPJ invalido", nameof(cpf)) : cpf;
            Address = address;
            Type = type;
            Status = CustomerStatus.Active;
        }

        public void Activate() => Status = CustomerStatus.Active;
        public void Deactivate() => Status = CustomerStatus.Inactive;
        public void SetAttention() => Status = CustomerStatus.Attention;
        public void SetVerified() => Status = CustomerStatus.Verified;
        public void Block() => Status = CustomerStatus.Blocked;

        public void UpdateDetails(string name, string email, string cpf, Address? address)
        {
            Name = string.IsNullOrWhiteSpace(name) ? throw new ArgumentException("Nome invalido", nameof(name)) : name;
            Email = string.IsNullOrWhiteSpace(email) ? throw new ArgumentException("Email invalido", nameof(email)) : email;
            Cpf = string.IsNullOrWhiteSpace(cpf) ? throw new ArgumentException("CPF/CNPJ invalido", nameof(cpf)) : cpf;
            Address = address;
        }

        public void EvaluateVerificationStatus(IEnumerable<Document> documents)
        {
            if (Status == CustomerStatus.Blocked || Status == CustomerStatus.Inactive) return;

            var docsList = documents?.ToList() ?? new List<Document>();
            var hasIdentity = docsList.Any(d => d.Type == DocumentType.Identity && !d.IsExpired());
            var hasAddressProof = docsList.Any(d => d.Type == DocumentType.AddressProof && !d.IsExpired());
            var hasSocialContract = docsList.Any(d => d.Type == DocumentType.SocialContract && !d.IsExpired());

            bool requirementsMet = false;
            if (Type == CustomerType.Individual)
            {
                // PF: Precisamos de Identidade (RG) e Comprovante de Endereço
                requirementsMet = hasIdentity && hasAddressProof;
            }
            else if (Type == CustomerType.LegalEntity)
            {
                // PJ: Precisamos de Identidade, Comprovante de Endereço e Contrato Social
                requirementsMet = hasIdentity && hasAddressProof && hasSocialContract;
            }

            if (requirementsMet)
            {
                SetVerified();
            }
            else if (docsList.Any(d => d.IsExpired()))
            {
                SetAttention();
            }
            else
            {
                Activate();
            }
        }
    }
}
