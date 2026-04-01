using System.Globalization;

namespace ClientManager.Domain.Core.Helpers
{
    public static class DocumentHelper
    {
        public static bool IsCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var cleanedCpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cleanedCpf.Length != 11)
                return false;

            if (cleanedCpf.Distinct().Count() == 1)
                return false;

            return HasValidCpfDigits(cleanedCpf);
        }

        public static bool IsCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            var cleanedCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (cleanedCnpj.Length != 14)
                return false;

            if (cleanedCnpj.Distinct().Count() == 1)
                return false;

            return HasValidCnpjDigits(cleanedCnpj);
        }

        private static bool HasValidCpfDigits(string cpf)
        {
            var tempCpf = cpf.Substring(0, 9);
            var sum = 0;
            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString(), CultureInfo.InvariantCulture) * multiplier1[i];

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            tempCpf += digit1;
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString(), CultureInfo.InvariantCulture) * multiplier2[i];

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf.EndsWith(digit1.ToString(CultureInfo.InvariantCulture) + digit2.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }

        private static bool HasValidCnpjDigits(string cnpj)
        {
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCnpj = cnpj.Substring(0, 12);
            var sum = 0;

            for (int i = 0; i < 12; i++)
                sum += int.Parse(tempCnpj[i].ToString(), CultureInfo.InvariantCulture) * multiplier1[i];

            var remainder = (sum % 11);
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            tempCnpj += digit1;
            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += int.Parse(tempCnpj[i].ToString(), CultureInfo.InvariantCulture) * multiplier2[i];

            remainder = (sum % 11);
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cnpj.EndsWith(digit1.ToString(CultureInfo.InvariantCulture) + digit2.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
        }
    }
}
