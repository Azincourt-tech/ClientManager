namespace ShopRavenDb.Domain.Core.Interfaces.Validators
{
    public interface ICpfValidator
    {
        bool IsValid(string cpf);
    }
}
