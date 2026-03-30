namespace ClientManager.Application.Dtos.User;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTimeOffset Expiration { get; set; }
    public UserDto User { get; set; } = null!;
}
