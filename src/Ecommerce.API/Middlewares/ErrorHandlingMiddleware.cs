using System.Text.Json;

namespace Ecommerce.API.Middlewares;

public class ErrorHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ErrorHandlingMiddleware> _logger;
  private readonly IHostEnvironment _env;

  public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
  {
    _next = next;
    _logger = logger;
    _env = env;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled Exception");

      context.Response.StatusCode = 500;
      context.Response.ContentType = "application/json";

      var errorResponse = new
      {
        StatusCode = 500,
        Message = "Đã xảy ra lỗi.",
        Detail = _env.IsDevelopment() ? ex.ToString() : null
      };

      await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
  }
}
