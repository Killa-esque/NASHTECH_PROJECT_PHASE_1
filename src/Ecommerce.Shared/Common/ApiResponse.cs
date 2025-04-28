using System.Text.Json.Serialization;

namespace Ecommerce.Shared.Common;
public class ApiResponse<T>
{
  [JsonPropertyName("status")]
  public bool Status { get; set; } = true;

  [JsonPropertyName("message")]
  public string Message { get; set; } = "Success";

  [JsonPropertyName("data")]
  public T Data { get; set; }

  public ApiResponse()
  {
  }

  public ApiResponse(T data, string message = "Success")
  {
    Data = data;
    Message = message;
  }

  public ApiResponse(string errorMessage)
  {
    Status = false;
    Message = errorMessage;
  }

  public static ApiResponse<T> Success(T data, string message = "Success") => new ApiResponse<T>(data, message);
  public static ApiResponse<T> Fail(string errorMessage) => new ApiResponse<T>(errorMessage);
}
