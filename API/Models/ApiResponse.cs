namespace API.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; init; }
        public string Message { get; init; }
        public T? Data { get; init; }
        //public IEnumerable<string>? Errors { get; init; }

        public static ApiResponse<T> Success(T data, string message = "Request successful")
            => new()
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };

        public static ApiResponse<T> Fail(
            string message,
            IEnumerable<string>? errors = null)
            => new()
            {
                IsSuccess = false,
                Message = message,
                //Errors = errors
            };
    }

}
