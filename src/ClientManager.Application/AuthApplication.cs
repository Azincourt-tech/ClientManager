using ClientManager.Application.Mappers;
using ClientManager.Domain.Core.Interfaces.Services;
using ClientManager.Domain.Core.Responses;
using FluentValidation;

namespace ClientManager.Application
{
    public class AuthApplication : IAuthApplication
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IValidator<Dtos.User.CreateUserDto> _createUserValidator;
        private readonly IValidator<Dtos.User.LoginDto> _loginValidator;

        // In-memory refresh token storage (in production, use a persistent store)
        private static readonly Dictionary<string, Guid> _refreshTokens = new();

        public AuthApplication(
            IUserService userService,
            ITokenService tokenService,
            IEmailService emailService,
            IValidator<Dtos.User.CreateUserDto> createUserValidator,
            IValidator<Dtos.User.LoginDto> loginValidator)
        {
            _userService = userService;
            _tokenService = tokenService;
            _emailService = emailService;
            _createUserValidator = createUserValidator;
            _loginValidator = loginValidator;
        }

        public async Task<ServiceResponse<Dtos.User.AuthResponseDto>> RegisterAsync(Dtos.User.CreateUserDto userDto)
        {
            var validationResult = await _createUserValidator.ValidateAsync(userDto).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail(firstError);
            }

            var existingUser = await _userService.GetUserByUsernameAsync(userDto.Username).ConfigureAwait(false);
            if (existingUser != null)
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("UsernameAlreadyExists");

            var existingEmail = await _userService.GetUserByEmailAsync(userDto.Email).ConfigureAwait(false);
            if (existingEmail != null)
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("EmailAlreadyExists");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = userDto.ToModel(passwordHash);

            await _userService.AddUserAsync(user).ConfigureAwait(false);

            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendWelcomeEmailAsync(user.Email, user.Username).ConfigureAwait(false);
                }
                catch
                {
                    // fire-and-forget: email failure should not block registration
                }
            });

            var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _refreshTokens[refreshToken] = user.Id;

            var authResponse = new Dtos.User.AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = user.ToDto(),
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            return ServiceResponse<Dtos.User.AuthResponseDto>.Ok(authResponse, "UserRegistered");
        }

        public async Task<ServiceResponse<Dtos.User.AuthResponseDto>> LoginAsync(Dtos.User.LoginDto loginDto)
        {
            var validationResult = await _loginValidator.ValidateAsync(loginDto).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail(firstError);
            }

            var user = await _userService.GetUserByUsernameAsync(loginDto.Username).ConfigureAwait(false);
            if (user == null)
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("InvalidCredentials");

            if (!user.IsActive)
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("UserInactive");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("InvalidCredentials");

            var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email, user.Role);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _refreshTokens[refreshToken] = user.Id;

            var authResponse = new Dtos.User.AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                User = user.ToDto(),
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            return ServiceResponse<Dtos.User.AuthResponseDto>.Ok(authResponse, "LoginSuccessful");
        }

        public async Task<ServiceResponse<Dtos.User.AuthResponseDto>> RefreshTokenAsync(string refreshToken)
        {
            if (!_refreshTokens.TryGetValue(refreshToken, out var userId))
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("InvalidRefreshToken");

            _refreshTokens.Remove(refreshToken);

            var user = await _userService.GetUserByIdAsync(userId).ConfigureAwait(false);
            if (user == null || !user.IsActive)
                return ServiceResponse<Dtos.User.AuthResponseDto>.Fail("UserNotFound");

            var newToken = _tokenService.GenerateToken(user.Id, user.Username, user.Email, user.Role);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            _refreshTokens[newRefreshToken] = user.Id;

            var authResponse = new Dtos.User.AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                User = user.ToDto(),
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            return ServiceResponse<Dtos.User.AuthResponseDto>.Ok(authResponse, "TokenRefreshed");
        }
    }
}
