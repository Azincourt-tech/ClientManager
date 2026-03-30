namespace ClientManager.Domain.Core.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(Guid userId, string username, string email, string role);

    string GenerateRefreshToken();
}
