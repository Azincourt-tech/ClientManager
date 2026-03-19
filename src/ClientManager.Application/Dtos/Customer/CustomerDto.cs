using ClientManager.Domain.Enums;

namespace ClientManager.Application.Dtos.Customer
{
    public class CustomerDto : CustomerBaseDto
    {
        public Guid Id { get; set; }
        public CustomerStatus Status { get; set; }
        public IEnumerable<DocumentDto> Documents { get; set; } = new List<DocumentDto>();
    }
}
