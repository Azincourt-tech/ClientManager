using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClientManager.Domain.Core.Responses
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Notifications { get; set; }

        public ServiceResponse()
        {
        }

        public ServiceResponse(T data, string? message = null)
        {
            Data = data;
            Success = true;
            Message = message;
        }

        public static ServiceResponse<T> Ok(T data, string? message = null) => new(data, message);

        public static ServiceResponse<T> Fail(string message, List<string>? notifications = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Notifications = notifications
            };
        }
        
        public static ServiceResponse<T> Fail(string message, string notification)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Notifications = new List<string> { notification }
            };
        }
    }
}

