using ShopRavenDb.Domain.Core.Interfaces.Validators;
using System.Linq;

namespace ShopRavenDb.Infrastructure.CrossCutting.Validators
{
    public class CpfValidator : ICpfValidator
    {
        public bool IsValid(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove non-digit characters
            var cleanedCpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cleanedCpf.Length != 11)
                return false;

            // Check for known invalid patterns (all digits same)
            if (cleanedCpf.Distinct().Count() == 1)
                return false;

            return HasValidDigits(cleanedCpf);
        }

        private static bool HasValidDigits(string cpf)
        {
            var tempCpf = cpf.Substring(0, 9);
            var sum = 0;

            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            tempCpf += digit1;
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf.EndsWith(digit1.ToString() + digit2.ToString());
        }
    }
}
