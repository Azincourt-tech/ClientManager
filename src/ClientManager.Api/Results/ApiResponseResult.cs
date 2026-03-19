using System.Collections.Generic;

namespace ClientManager.Api.Results
{
    /// <summary>
    /// Wrapper for standard 200 OK ServiceResponse.
    /// </summary>
    public class ApiOkResult<T>
    {
        public bool Success { get; set; } = true;

        public T? Data { get; set; }

        public ApiOkResult(T? data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Wrapper for standard 400 Bad Request ServiceResponse.
    /// </summary>
    public class ApiBadRequestResult
    {
        public bool Success { get; set; }

        public IReadOnlyCollection<string> Notifications { get; set; }

        public ApiBadRequestResult(IReadOnlyCollection<string>? notifications)
        {
            Success = false;
            Notifications = notifications ?? new List<string>();
        }
    }
}
