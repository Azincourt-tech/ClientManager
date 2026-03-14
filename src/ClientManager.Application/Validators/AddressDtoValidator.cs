using FluentValidation;
using ClientManager.Application.Dtos.Customer;

namespace ClientManager.Application.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            RuleFor(x => x.Street).NotEmpty().WithMessage("StreetRequired");
            RuleFor(x => x.Number).GreaterThan(0).WithMessage("InvalidNumber");
            RuleFor(x => x.City).NotEmpty().WithMessage("CityRequired");
            RuleFor(x => x.State).NotEmpty().WithMessage("StateRequired");
            RuleFor(x => x.PostalCode).NotEmpty().WithMessage("PostalCodeRequired");
        }
    }
}
