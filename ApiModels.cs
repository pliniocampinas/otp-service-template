namespace otp_service_template;


public class RegisterUserRequest()
{
  public string FullName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
}

public class StartValidationRequest()
{
  public string Email { get; set; } = string.Empty;
}

public class ConfirmValidationRequest()
{
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}

public class ApiResponse<T>()
{
  public string? ErrorMessage { get; set; } = string.Empty;
  public T? Content { get; set; }
}

public class ApiResponse()
{
  public string? ErrorMessage { get; set; } = string.Empty;
}