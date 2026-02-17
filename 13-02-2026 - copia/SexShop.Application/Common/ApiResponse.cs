namespace SexShop.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse() { }

        public ApiResponse(T data, string message = null)
        {
            Succeeded = true;
            Message = message ?? "Success";
            Data = data;
        }

        public ApiResponse(string message)
        {
            Succeeded = false;
            Message = message;
        }

        public ApiResponse(List<string> errors, string message = "Validation Failed")
        {
            Succeeded = false;
            Errors = errors;
            Message = message;
        }
    }
}
