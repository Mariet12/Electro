using System.Text.Json.Serialization;

namespace Electro.Core.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; }
        public ApiResponse()
        {
        }
        public ApiResponse(int statusCode, string message, object data = null)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
        }

        // Provides default messages for common HTTP status codes
        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "You are not Authorized",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => null,
            };
        }
    }

}
