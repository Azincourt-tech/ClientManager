using ClientManager.Domain.Enums;

namespace ClientManager.Domain.Model
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public DateTimeOffset BirthDate { get; private set; }
        public Address? Address { get; private set; }
        public string Document { get; private set; } = null!; // Can be CNPJ if LegalEntity
        public CustomerType Type { get; private set; }
        public CustomerStatus Status { get; private set; }
        public string? Cpf => Type == CustomerType.NaturalPerson ? Document : null;
        public string? Cnpj => Type == CustomerType.LegalEntity ? Document : null;

        // Construtor vazio para o RavenDB (Desserialização)
        private Customer() { }

        public Customer(string name, string email, DateTimeOffset birthDate, string document, CustomerType type, Address? address = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            BirthDate = birthDate;
            Document = CleanDocument(document);
            Address = address;
            Type = type;
            Status = CustomerStatus.Pending;
        }

        public void Activate() => Status = CustomerStatus.Active;
        public void Deactivate() => Status = CustomerStatus.Inactive;
        public void SetAttention() => Status = CustomerStatus.Attention;
        public void SetVerified() => Status = CustomerStatus.Verified;
        public void SetPending() => Status = CustomerStatus.Pending;

        public void UpdateDetails(string name, string email, string document, Address? address)
        {
            Name = name;
            Email = email;
            Document = CleanDocument(document);
            Address = address;
        }

        private string CleanDocument(string document)
        {
            if (string.IsNullOrWhiteSpace(document)) return string.Empty;
            return new string(document.Where(c => char.IsDigit(c)).ToArray());
        }

        public void EvaluateVerificationStatus(IEnumerable<Document> documents)
        {
            if (Status == CustomerStatus.Inactive) return;

            var docsList = documents?.ToList() ?? new List<Document>();
            
            // Se não tem nenhum documento, volta/mantém como Pendente
            if (!docsList.Any())
            {
                SetPending();
                return;
            }

            var hasIdentity = docsList.Any(d => d.Type == DocumentType.Identity && !d.IsExpired());
            var hasAddressProof = docsList.Any(d => d.Type == DocumentType.AddressProof && !d.IsExpired());
            var hasSocialContract = docsList.Any(d => d.Type == DocumentType.SocialContract && !d.IsExpired());

            bool requirementsMet = false;
            if (Type == CustomerType.NaturalPerson)
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
            else
            {
                // Se tem documentos mas não cumpre os requisitos (faltando algum ou expirado), fica em Atenção
                SetAttention();
            }
        }
    }
}

