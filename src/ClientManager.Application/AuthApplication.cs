using ClientManager.Application.Dtos.User;
using ClientManager.Application.Interfaces;
using ClientManager.Application.Mappers;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Core.Responses;
using Microsoft.Extensions.Configuration;

namespace ClientManager.Application;

public class AuthApplication : IAuthApplication
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthApplication(IUserService userService, ITokenService tokenService, IConfiguration configuration)
    {
        _userService = userService;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<ServiceResponse<AuthResponseDto>> RegisterAsync(CreateUserDto createUserDto)
    {
        var existingUser = await _userService.GetUserByUsernameAsync(createUserDto.Username).ConfigureAwait(false);
        if (existingUser != null)
            return ServiceResponse<AuthResponseDto>.Fail("UsernameAlreadyExists");

        var existingEmail = await _userService.GetUserByEmailAsync(createUserDto.Email).ConfigureAwait(false);
        if (existingEmail != null)
            return ServiceResponse<AuthResponseDto>.Fail("EmailAlreadyExists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        var user = createUserDto.ToModel(passwordHash);

        await _userService.AddUserAsync(user).ConfigureAwait(false);

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expirationMinutes = double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes),
            User = user.ToDto()
        };

        return ServiceResponse<AuthResponseDto>.Ok(response, "UserRegistered");
    }

    public async Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await _userService.GetUserByUsernameAsync(loginDto.Username).ConfigureAwait(false);

        if (user == null)
            return ServiceResponse<AuthResponseDto>.Fail("InvalidCredentials");

        if (!user.IsActive)
            return ServiceResponse<AuthResponseDto>.Fail("UserInactive");

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return ServiceResponse<AuthResponseDto>.Fail("InvalidCredentials");

        var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email, user.Role.ToString());
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expirationMinutes = double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes),
            User = user.ToDto()
        };

        return ServiceResponse<AuthResponseDto>.Ok(response, "LoginSuccessful");
    }

    public async Task<ServiceResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return ServiceResponse<AuthResponseDto>.Fail("RefreshTokenRequired");

        var newToken = _tokenService.GenerateRefreshToken();

        var response = new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newToken,
            Expiration = DateTimeOffset.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60"))
        };

        return ServiceResponse<AuthResponseDto>.Ok(response, "TokenRefreshed");
    }
}
