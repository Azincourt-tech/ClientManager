using FluentValidation;

namespace ShopRavenDb.Application.Validators
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(x => x).
                NotNull().
                WithMessage("File cannot be null.");

            RuleFor(x => x.Length).
                GreaterThan(0).
                WithMessage("File size must be greater than 0 bytes.");

            RuleFor(x => x.FileName).
                NotEmpty().
                WithMessage("File name cannot be empty.");
        }
    }
}
