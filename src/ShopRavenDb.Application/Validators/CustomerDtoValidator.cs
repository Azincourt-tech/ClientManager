using FluentValidation;
using ShopRavenDb.Domain.Core.Interfaces.Validators;
using ShopRavenDb.Domain.Enums;

namespace ShopRavenDb.Application.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator(
            ICpfValidator cpfValidator,
            ICnpjValidator cnpjValidator)
        {
            RuleFor(x => x.Name).
                NotEmpty().
                WithMessage("NameRequired");

            RuleFor(x => x.Email).
                NotEmpty().
                WithMessage("EmailRequired")
                .EmailAddress().
                WithMessage("InvalidEmail");

            RuleFor(x => x.Document)
                .NotEmpty()
                .WithMessage("DocumentRequired")
                .Must((dto, doc) =>
                {
                    var cleaned = new string(doc?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                    return dto.Type == CustomerType.NaturalPerson ? cpfValidator.IsValid(cleaned) : true;
                })
                .WithMessage("InvalidCPF")
                .Must((dto, doc) =>
                {
                    var cleaned = new string(doc?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
                    return dto.Type == CustomerType.LegalEntity ? cnpjValidator.IsValid(cleaned) : true;
                })
                .WithMessage("InvalidCNPJ");

            RuleFor(x => x.BirthDate).
                NotEmpty().
                WithMessage("BirthDateRequired");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("InvalidCustomerType");
        }
    }
}
