using ClientManager.Api.Results;
using ClientManager.Domain.Core.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ClientManager.Api.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UserController : MainController
{
    private readonly IUserApplication _userApplication;

    public UserController(IUserApplication userApplication)
    {
        _userApplication = userApplication;
    }

    /// <summary>
    /// Creates a new user (admin only).
    /// </summary>
    /// <param name="createUserDto">The user data to be created.</param>
    /// <returns>A service response containing the created user.</returns>
    [HttpPost]
    [EndpointSummary("Creates a new user (admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
    {
        var response = await _userApplication.CreateUserAsync(createUserDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Gets a user by ID (admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A service response containing the user data.</returns>
    [HttpGet("{id}")]
    [EndpointSummary("Gets a user by ID (admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var response = await _userApplication.GetUserByIdAsync(id).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Gets all active users (admin only).
    /// </summary>
    /// <returns>A service response containing a list of users.</returns>
    [HttpGet]
    [EndpointSummary("Gets all active users (admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userApplication.GetUsersAsync().ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Updates an existing user (admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="updateUserDto">The updated user data.</param>
    /// <returns>A service response containing the updated user.</returns>
    [HttpPut("{id}")]
    [EndpointSummary("Updates an existing user (admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, CreateUserDto updateUserDto)
    {
        var response = await _userApplication.UpdateUserAsync(id, updateUserDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Deactivates a user (admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A service response confirming the deactivation.</returns>
    [HttpDelete("{id}")]
    [EndpointSummary("Deactivates a user (admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var response = await _userApplication.DeleteUserAsync(id).ConfigureAwait(false);
        return ServiceResponse(response);
    }
}
