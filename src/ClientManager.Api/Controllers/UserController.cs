using ClientManager.Api.Results;
using ClientManager.Application.Dtos.User;
using ClientManager.Domain.Core.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ClientManager.Api.Controllers;

/// <summary>
/// User management endpoints (Admin only).
/// </summary>
[Route("api/[controller]")]
[Authorize(Policy = "AdminOnly")]
public class UserController : MainController
{
    private readonly IUserApplication _userApplication;
    private readonly Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> _localizer;

    public UserController(IUserApplication userApplication, Microsoft.Extensions.Localization.IStringLocalizer<SharedResource> localizer)
    {
        _userApplication = userApplication;
        _localizer = localizer;
    }

    /// <summary>
    /// Creates a new user (Admin only).
    /// </summary>
    /// <param name="userDto">The user data to create.</param>
    /// <returns>A service response containing the new user ID.</returns>
    [HttpPost("user", Name = "add-user")]
    [EndpointSummary("Creates a new user (Admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUser(CreateUserDto userDto)
    {
        var response = await _userApplication.AddUserAsync(userDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Updates an existing user (Admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="userDto">The updated user data.</param>
    /// <returns>A service response indicating success or failure of the update.</returns>
    [HttpPut("user/{id}", Name = "update-user")]
    [EndpointSummary("Updates an existing user (Admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, CreateUserDto userDto)
    {
        var response = await _userApplication.UpdateUserAsync(id, userDto).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Removes a user by their ID (Admin only).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A service response confirming the deletion.</returns>
    [HttpDelete("user/{userId}", Name = "delete-user-by-id")]
    [EndpointSummary("Removes a user by their ID (Admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteUserById(Guid userId)
    {
        var response = await _userApplication.DeleteUserByIdAsync(userId).ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Retrieves a list of all registered users (Admin only).
    /// </summary>
    /// <returns>A list of users wrapped in a service response.</returns>
    [HttpGet("users", Name = "get-users")]
    [EndpointSummary("Retrieves a list of all registered users (Admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var response = await _userApplication.GetUsersAsync().ConfigureAwait(false);
        return ServiceResponse(response);
    }

    /// <summary>
    /// Gets the details of a specific user by their ID (Admin only).
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The user data if found.</returns>
    [HttpGet("users/{userId}", Name = "get-user-by-id")]
    [EndpointSummary("Gets the details of a specific user by their ID (Admin only).")]
    [ProducesResponseType(typeof(ApiOkResult<UserDto?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiBadRequestResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var response = await _userApplication.GetUserByIdAsync(userId).ConfigureAwait(false);
        return ServiceResponse(response);
    }
}
