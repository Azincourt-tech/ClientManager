using ClientManager.Domain.Core.Interfaces.Validators;
using System.Text.RegularExpressions;

namespace ClientManager.Infrastructure.CrossCutting.Validators
{
    public class EmailValidator : IEmailValidator
    {
        // Regex para validação de email, conforme RFC 5322
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public bool IsValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return EmailRegex.IsMatch(email);
        }
    }
}

