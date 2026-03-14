namespace ShopRavenDb.Domain.Core.Interfaces.Validators
{
    public interface ICnpjValidator
    {
        bool IsValid(string cnpj);
    }
}
