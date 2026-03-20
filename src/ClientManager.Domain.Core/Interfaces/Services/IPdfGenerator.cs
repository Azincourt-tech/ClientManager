namespace ClientManager.Domain.Core.Interfaces.Services;

public interface IPdfGenerator
{
    Task<byte[]> GenerateWelcomeKitAsync(Guid customerId, string name);
}
