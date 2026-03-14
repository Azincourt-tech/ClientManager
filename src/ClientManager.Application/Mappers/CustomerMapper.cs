using ClientManager.Application.Dtos.Customer;
using ClientManager.Application.Dtos.Document;
using ClientManager.Domain.Model;

namespace ClientManager.Application.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToDto(this Customer customer, IEnumerable<Document>? documents = null)
        {
            if (customer == null) return null!;

            return new CustomerDto
            {
                Id = customer.Id,
                Name = customer.Name,
                Email = customer.Email,
                BirthDate = customer.BirthDate,
                Document = customer.Document,
                Type = customer.Type,
                Status = customer.Status,
                Address = customer.Address.ToDto(),
                Documents = documents?.Select(d => d.ToDto()) ?? new List<DocumentDto>()
            };
        }

        public static AddressDto ToDto(this Address? address)
        {
            if (address == null) return null!;

            return new AddressDto
            {
                Street = address.Street,
                Number = address.Number,
                Complement = address.Complement,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode
            };
        }

        public static Customer ToModel(this CreateCustomerDto dto)
        {
            if (dto == null) return null!;

            return new Customer(
                dto.Name,
                dto.Email,
                dto.BirthDate,
                dto.Document,
                dto.Type,
                dto.Address.ToModel()
            );
        }

        public static Address ToModel(this AddressDto addressDto)
        {
            if (addressDto == null) return null!;

            return new Address(
                addressDto.Street,
                addressDto.Number,
                addressDto.Complement,
                addressDto.City,
                addressDto.State,
                addressDto.PostalCode
            );
        }
    }
}
