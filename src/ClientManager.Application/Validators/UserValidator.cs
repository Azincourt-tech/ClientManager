using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Helpers;
using FluentValidation;

namespace ClientManager.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
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

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("InvalidUserRole");
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("UsernameRequired");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("PasswordRequired");
    }
}
