using Microsoft.AspNetCore.Diagnostics;

namespace otp_service_template;

public class ApiErrorHandler : IExceptionHandler
{
  private readonly ILogger<ApiErrorHandler> Logger;
  public ApiErrorHandler(ILogger<ApiErrorHandler> logger)
  {
    Logger = logger;
  }
  public ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
  {
    var exceptionMessage = exception.Message;
    Logger.LogError(
      "Error Message: {exceptionMessage}, Time of occurrence {time}",
      exceptionMessage, DateTime.UtcNow);
    
    return ValueTask.FromResult(false);
  }
}