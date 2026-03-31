using ClientManager.Api.Results;
using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ClientManager.Api.Controllers;

/// <summary>
/// Authentication endpoints for user registration and login.
/// </summary>
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : MainController
{
    private readonly IAuthApplication _authApplication;
    private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

    public AuthController(IAuthApplication authApplication, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
    {
        _authApplication = authApplication;
        _localizer = localizer;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="userDto">The user data to register.</param>
    /// <returns>A service response containing the user info (without token).</returns>
    [HttpPost("register", Name = "auth-register")]
    [EndpointSummary("Registers a new user in the system.")]
    [ProducesResponseType(typeof(ApiOkResult<RegisterResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(CreateUserDto userDto)
    {
        var response = await _authApplication.RegisterAsync(userDto).ConfigureAwait(false);

        if (!response.Success || response.Data == null)
        {
            return ServiceResponse(new ServiceResponse<RegisterResponseDto>
            {
                Success = false,
                Message = response.Message,
                Notifications = response.Notifications
            });
        }

        var registerResponse = new RegisterResponseDto
        {
            User = response.Data.User,
            ExpiresAt = response.Data.ExpiresAt
        };

        return ServiceResponse(new ServiceResponse<RegisterResponseDto>(registerResponse, response.Message));
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A service response containing the authentication token and user info.</returns>
    [HttpPost("login", Name = "auth-login")]
    [EndpointSummary("Authenticates a user and returns a JWT token.")]
    [ProducesResponseType(typeof(ApiOkResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var response = await _authApplication.LoginAsync(loginDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Refreshes an expired JWT token using a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>A service response containing the new authentication token.</returns>
    [HttpPost("refresh", Name = "auth-refresh")]
    [EndpointSummary("Refreshes an expired JWT token using a refresh token.")]
    [ProducesResponseType(typeof(ApiOkResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
    {
        var response = await _authApplication.RefreshTokenAsync(refreshToken).ConfigureAwait(false);
        return ServiceResponse(response);
    }
}
