using ClientManager.Domain.Core.Helpers;
using ClientManager.Domain.Enums;
using FluentValidation;

namespace ClientManager.Application.Validators
{
    public class CustomerValidator : AbstractValidator<CustomerBaseDto>
    {
        public CustomerValidator()
        {


            // Regras Comuns
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("NameRequired");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("EmailRequired")
                .Must(EmailHelper.IsValid)
                .WithMessage("InvalidEmail");

            RuleFor(x => x.Document)
                .NotEmpty()
                .WithMessage("DocumentRequired")
                .Must((dto, doc) => dto.Type == CustomerType.NaturalPerson ? DocumentHelper.IsCpf(doc) : true).WithMessage("InvalidCPF")
                .Must((dto, doc) => dto.Type == CustomerType.LegalEntity ? DocumentHelper.IsCnpj(doc) : true).WithMessage("InvalidCNPJ");

            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage("BirthDateRequired");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("InvalidCustomerType");

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("AddressRequired")
                .SetValidator(new AddressDtoValidator());
        }
    }
}
