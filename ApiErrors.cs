namespace otp_service_template;

public enum ApiError
{
  None,
  InvalidEmail,
}

public static class ApiErrors
{
  private static readonly Dictionary<ApiError, string> ErrorMessages = new Dictionary<ApiError, string>
  {
    { ApiError.None, "Unknown Error" },
    { ApiError.InvalidEmail, "Invalid Email" }
  };

  public static ApiResponse AsApiResponse(this ApiError error)
  {
    var success = ErrorMessages.TryGetValue(error, out var message);

    if (success == false)
      throw new Exception("Unmapped error");
    
    return new ApiResponse()
    {
      ErrorMessage = message
    };
  }
}