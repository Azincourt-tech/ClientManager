using FluentValidation;

namespace ShopRavenDb.Application.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        public CustomerDtoValidator()
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
                WithMessage("CPF is required");

            RuleFor(x => x.BirthDate).
                NotEmpty().
                WithMessage("Birth date is required");
        }
    }
}
