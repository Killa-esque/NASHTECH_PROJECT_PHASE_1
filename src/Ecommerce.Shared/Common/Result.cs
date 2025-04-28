namespace Ecommerce.Shared.Common;

public class Result
{
    public bool IsSuccess { get; }
    public Error Error { get; }
    public string? Message { get; }

    protected Result(bool isSuccess, Error error, string? message = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    public static Result Success(string? message = null) => new(true, Error.None, message);
    public static Result Failure(Error error, string? message = null) => new(false, error, message);

    public static Result<T> Success<T>(T data, string? message = null) => new(true, Error.None, data, message);
    public static Result<T> Failure<T>(Error error, string? message = null) => new(false, error, default, message);
}

