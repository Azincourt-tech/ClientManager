using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Responses;

namespace ClientManager.Application.Interfaces;

public interface IAuthApplication
{
    Task<ServiceResponse<AuthResponseDto>> RegisterAsync(CreateUserDto createUserDto);

    Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);

    Task<ServiceResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
}
