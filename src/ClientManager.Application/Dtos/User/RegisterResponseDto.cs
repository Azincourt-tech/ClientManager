namespace ClientManager.Application.Dtos.User
{
    public class RegisterResponseDto
    {
        public UserDto User { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
