namespace otp_service_template.Services;

public enum UserStatus
{
  None,
  Created,
  Active,
  Deactivated
}

public class User
{
  public string Email { get; set; } = string.Empty;
  public UserStatus Status { get; set; }
}