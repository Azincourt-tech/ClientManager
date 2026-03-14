using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ShopRavenDb.Domain.Core.Responses
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }

        public ServiceResponse()
        {
        }

        public ServiceResponse(T data, string message = "")
        {
            Data = data;
            Success = true;
            Message = message;
        }

        public static ServiceResponse<T> Ok(T data, string message = "") => new(data, message);

        public static ServiceResponse<T> Fail(string message, List<string>? errors = null)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
        
        public static ServiceResponse<T> Fail(string message, string error)
        {
            return new ServiceResponse<T>
            {
                Success = false,
                Message = message,
                Errors = new List<string> { error }
            };
        }
    }
}
