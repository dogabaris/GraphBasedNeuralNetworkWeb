namespace ThesisWebApplication.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static Result AsSuccess()
        {
            return new Result { IsSuccess = true };
        }

        public static Result AsSuccessWithMessage(string message)
        {
            return new Result { IsSuccess = true, Message = message };
        }

        public static Result AsError(string errorMessage)
        {
            return new Result { Message = errorMessage };
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }
        public new static Result<T> AsError(string errorMessage)
        {
            return new Result<T> { Message = errorMessage };
        }

        public static Result<T> AsSuccess(T data)
        {
            return new Result<T> { IsSuccess = true, Data = data };
        }

        public static Result<T> AsSuccessWithMessage(T data, string message)
        {
            return new Result<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message
            };
        }
    }
}