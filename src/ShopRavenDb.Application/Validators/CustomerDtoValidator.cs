using FluentValidation;

namespace ShopRavenDb.Application.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator(
            ShopRavenDb.Domain.Core.Interfaces.Validators.ICpfValidator cpfValidator,
            ShopRavenDb.Domain.Core.Interfaces.Validators.ICnpjValidator cnpjValidator)
        {
            RuleFor(x => x.Name).
                NotEmpty().
                WithMessage("Name is required");

            RuleFor(x => x.Email).
                NotEmpty().
                EmailAddress().
                WithMessage("A valid email is required");

            RuleFor(x => x.Document)
                .NotEmpty()
                .WithMessage("CPF/CNPJ is required")
                .Must((dto, doc) => dto.Type == ShopRavenDb.Domain.Enums.CustomerType.Individual ? cpfValidator.IsValid(doc) : true)
                .WithMessage("Invalid CPF number")
                .Must((dto, doc) => dto.Type == ShopRavenDb.Domain.Enums.CustomerType.LegalEntity ? cnpjValidator.IsValid(doc) : true)
                .WithMessage("Invalid CNPJ number");

            RuleFor(x => x.BirthDate).
                NotEmpty().
                WithMessage("Birth date is required");
                
            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Invalid customer type");
        }
    }
}
