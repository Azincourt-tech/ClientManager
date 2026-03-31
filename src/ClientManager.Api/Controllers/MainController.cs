using ClientManager.Api.Results;
using ClientManager.Domain.Core.Responses;
using Microsoft.AspNetCore.Authorization;

namespace ClientManager.Api.Controllers
{
    [Authorize(Policy = "AllRoles")]
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        /// <summary>
        /// Envelops a ServiceResponse into a standardized IActionResult.
        /// </summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <param name="response">The service response to evaluate.</param>
        /// <returns>An IActionResult with the appropriate HTTP status code.</returns>
        protected IActionResult ServiceResponse<T>(ServiceResponse<T> response)
        {
            if (response.Success)
            {
                return Ok(new ApiOkResult<T>(response.Data));
            }

            // In case of error, we return BadRequest with the notifications
            // We can also include the main message in the notifications list if needed
            var notifications = response.Notifications ?? new List<string>();
            if (!string.IsNullOrEmpty(response.Message) && !notifications.Contains(response.Message))
            {
                notifications.Insert(0, response.Message);
            }

            return BadRequest(new ApiBadRequestResult(notifications));
        }
    }
}
