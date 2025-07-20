namespace Online_Learning.Models.DTOs.Response.Admin
{
    public class AdminApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static AdminApiResponse<T> SuccessResult(T data, string message = "Thành công")
        {
            return new AdminApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static AdminApiResponse<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new AdminApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}

