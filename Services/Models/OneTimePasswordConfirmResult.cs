namespace otp_service_template.Services;

public enum ConfirmationStatus
{
  None,
  Authorized
}

public class OneTimePasswordConfirmResult
{
  public ConfirmationStatus Status { get; set; }
  public string Token { get; set; } = string.Empty;
}