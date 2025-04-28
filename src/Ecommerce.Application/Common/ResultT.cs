namespace Ecommerce.Application.Common;
public class Result<T> : Result
{
  public T? Data { get; }

  internal Result(bool isSuccess, Error error, T? data, string? message = null)
      : base(isSuccess, error, message)
  {
    Data = data;
  }
}
