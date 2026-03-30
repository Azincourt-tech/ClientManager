using ClientManager.Domain.Core.Helpers;
using FluentValidation;

namespace ClientManager.Application.Validators
{
    public class CreateUserValidator : AbstractValidator<Dtos.User.CreateUserDto>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("UsernameRequired")
                .MinimumLength(3)
                .WithMessage("UsernameMinLength")
                .MaximumLength(50)
                .WithMessage("UsernameMaxLength");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("EmailRequired")
                .Must(EmailHelper.IsValid)
                .WithMessage("InvalidEmail");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("PasswordRequired")
                .MinimumLength(6)
                .WithMessage("PasswordMinLength");
        }
    }

    public class LoginValidator : AbstractValidator<Dtos.User.LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("UsernameRequired");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("PasswordRequired");
        }
    }
}
