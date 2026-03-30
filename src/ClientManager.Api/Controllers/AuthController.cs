using ClientManager.Api.Results;
using ClientManager.Domain.Core.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ClientManager.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : MainController
{
    private readonly IAuthApplication _authApplication;

    public AuthController(IAuthApplication authApplication)
    {
        _authApplication = authApplication;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="createUserDto">The user data to be registered.</param>
    /// <returns>A service response containing authentication data.</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [EndpointSummary("Registers a new user in the system.")]
    [ProducesResponseType(typeof(ApiOkResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(CreateUserDto createUserDto)
    {
        var response = await _authApplication.RegisterAsync(createUserDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A service response containing authentication data.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Authenticates a user and returns a JWT token.")]
    [ProducesResponseType(typeof(ApiOkResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var response = await _authApplication.LoginAsync(loginDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Refreshes an expired token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>A service response containing new authentication data.</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [EndpointSummary("Refreshes an expired token.")]
    [ProducesResponseType(typeof(ApiOkResult<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var response = await _authApplication.RefreshTokenAsync(refreshToken).ConfigureAwait(false);
        return ServiceResponse(response);
    }
}
