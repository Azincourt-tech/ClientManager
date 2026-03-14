using FluentValidation;

namespace ShopRavenDb.Application.Validators
{
    public class FileValidator : AbstractValidator<IFormFile>
    {
        public FileValidator()
        {
            RuleFor(x => x).
                NotNull().
                WithMessage("FileNotFound");

            RuleFor(x => x.Length).
                GreaterThan(0).
                WithMessage("FileEmpty");

            RuleFor(x => x.FileName).
                NotEmpty().
                WithMessage("FileNameEmpty");
        }
    }
}
