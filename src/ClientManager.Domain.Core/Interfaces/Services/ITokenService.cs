using ClientManager.Domain.Enums;

namespace ClientManager.Domain.Core.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(Guid userId, string username, string email, UserRole role);
        string GenerateRefreshToken();
        Guid? ValidateToken(string token);
    }
}
