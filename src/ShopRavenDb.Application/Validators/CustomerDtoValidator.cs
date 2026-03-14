using FluentValidation;

namespace ShopRavenDb.Application.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator(ShopRavenDb.Domain.Core.Interfaces.Validators.ICpfValidator cpfValidator)
        {
            RuleFor(x => x.Name).
                NotEmpty().
                WithMessage("Name is required");

            RuleFor(x => x.Email).
                NotEmpty().
                EmailAddress().
                WithMessage("A valid email is required");

            RuleFor(x => x.Cpf).
                NotEmpty().
                WithMessage("CPF is required")
                .Must(cpfValidator.IsValid)
                .WithMessage("Invalid CPF number");

            RuleFor(x => x.BirthDate).
                NotEmpty().
                WithMessage("Birth date is required");
        }
    }
}
