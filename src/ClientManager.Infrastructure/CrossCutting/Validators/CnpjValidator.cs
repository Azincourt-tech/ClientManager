using ClientManager.Domain.Core.Interfaces.Validators;
using System.Linq;

namespace ClientManager.Infrastructure.CrossCutting.Validators
{
    public class CnpjValidator : ICnpjValidator
    {
        public bool IsValid(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            var cleanedCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (cleanedCnpj.Length != 14)
                return false;

            if (cleanedCnpj.Distinct().Count() == 1)
                return false;

            return HasValidDigits(cleanedCnpj);
        }

        private static bool HasValidDigits(string cnpj)
        {
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCnpj = cnpj.Substring(0, 12);
            var sum = 0;

            for (int i = 0; i < 12; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];

            var remainder = (sum % 11);
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            tempCnpj += digit1;
            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];

            remainder = (sum % 11);
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cnpj.EndsWith(digit1.ToString() + digit2.ToString());
        }
    }
}

