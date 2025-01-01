namespace otp_service_template.Services;

public interface IEmailService
{
  Task Send(ValidationEmail email);
}