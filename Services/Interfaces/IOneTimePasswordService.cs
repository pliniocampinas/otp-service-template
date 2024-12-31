namespace otp_service_template.Services;

public interface IOneTimePasswordService
{
  Task RequestValidation(string email);
  Task<OneTimePasswordConfirmResult> ConfirmValidation(string email, string password);
}